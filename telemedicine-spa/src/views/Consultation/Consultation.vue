<template>
  <div class="mx-auto max-w-6xl p-4 sm:p-6" aria-live="polite">
    <header class="mb-5 flex flex-wrap items-center justify-between gap-3">
      <div><h1 class="text-2xl font-bold text-slate-900 dark:text-white">Video consultation</h1><p class="text-sm text-slate-600 dark:text-slate-300">{{ contextText }}</p></div>
      <span class="rounded-full px-3 py-1 text-sm font-semibold" :class="stateClass">{{ stateLabel }}</span>
    </header>
    <div v-if="error" role="alert" class="mb-4 rounded-lg bg-red-50 p-4 text-red-800 dark:bg-red-950 dark:text-red-200">{{ error }}</div>
    <section v-if="!joined" class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm dark:border-slate-700 dark:bg-slate-900">
      <h2 class="text-lg font-semibold">Ready to join?</h2>
      <p class="mt-2 text-slate-600 dark:text-slate-300">Your browser will ask for camera and microphone permission. Media is sent peer-to-peer and is not recorded by this application.</p>
      <button class="btn-primary mt-5" type="button" :disabled="busy || ended" @click="join">Allow camera and microphone</button>
    </section>
    <template v-else>
      <section class="grid gap-4 lg:grid-cols-3">
        <div class="relative min-h-96 overflow-hidden rounded-2xl bg-black lg:col-span-2">
          <video ref="remoteVideo" autoplay playsinline class="h-full w-full object-cover" aria-label="Remote participant video"></video>
          <p v-if="!remoteMedia" class="absolute inset-0 grid place-items-center text-center text-white">Waiting for the other participant and media connection…</p>
        </div>
        <div class="relative min-h-52 overflow-hidden rounded-2xl bg-slate-950">
          <video ref="localVideo" autoplay muted playsinline class="h-full w-full object-cover" aria-label="Your local video preview"></video>
          <span class="absolute bottom-3 left-3 rounded bg-black/60 px-2 py-1 text-xs text-white">You</span>
        </div>
      </section>
      <div class="mt-5 flex flex-wrap justify-center gap-3">
        <button type="button" class="rounded-lg bg-slate-700 px-4 py-3 text-white" :aria-pressed="muted" @click="toggleMicrophone">{{ muted ? 'Unmute microphone' : 'Mute microphone' }}</button>
        <button type="button" class="rounded-lg bg-slate-700 px-4 py-3 text-white" :aria-pressed="!cameraEnabled" @click="toggleCamera">{{ cameraEnabled ? 'Turn camera off' : 'Turn camera on' }}</button>
        <button type="button" class="rounded-lg bg-amber-600 px-4 py-3 text-white" @click="leave(false)">Leave</button>
        <button type="button" class="rounded-lg bg-red-700 px-4 py-3 text-white" @click="leave(true)">End consultation</button>
      </div>
    </template>
  </div>
</template>

<script>
import * as signalR from '@microsoft/signalr'
import { getConsultation, joinConsultation, endConsultation, getConsultationIceServers } from '@/api'

const HUB_URL = import.meta.env.VITE_CONSULTATION_HUB_URL || '/consultation-hub'

export default {
  name: 'ConsultationView',
  data: () => ({ sessionId: '', session: null, connection: null, peer: null, localStream: null, pendingCandidates: [], joined: false, busy: false, muted: false, cameraEnabled: true, remoteMedia: false, state: 'ready', error: '', ended: false }),
  computed: {
    contextText() { return this.session ? `${this.session.patientName} with ${this.session.physicianName}` : 'Loading authorized appointment…' },
    stateLabel() { return ({ ready: 'Ready', connecting: 'Connecting', connected: 'Connected', reconnecting: 'Reconnecting', failed: 'Connection failed', ended: 'Ended' })[this.state] || this.state },
    stateClass() { return this.state === 'connected' ? 'bg-emerald-100 text-emerald-800' : this.state === 'failed' ? 'bg-red-100 text-red-800' : 'bg-slate-200 text-slate-800' },
  },
  async mounted() {
    this.sessionId = this.$route.params.sessionId
    try { this.session = await getConsultation(this.sessionId); if (['Completed', 'Cancelled', 'Failed'].includes(this.session.status)) { this.ended = true; this.state = 'ended' } }
    catch (e) { this.error = e.message || 'You are not authorized for this consultation.'; this.state = 'failed' }
  },
  beforeUnmount() { this.cleanup() },
  methods: {
    async join() {
      this.busy = true; this.error = ''; this.state = 'connecting'
      try {
        if (!navigator.mediaDevices?.getUserMedia) throw new Error('Camera and microphone are not available in this browser or security context.')
        this.localStream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true })
        this.joined = true
        await this.$nextTick()
        this.$refs.localVideo.srcObject = this.localStream
        this.session = await joinConsultation(this.sessionId)
        const iceServers = await getConsultationIceServers(this.sessionId)
        this.peer = new RTCPeerConnection({ iceServers })
        this.localStream.getTracks().forEach(track => this.peer.addTrack(track, this.localStream))
        this.peer.ontrack = ({ streams }) => { this.$refs.remoteVideo.srcObject = streams[0]; this.remoteMedia = true }
        this.peer.onicecandidate = ({ candidate }) => { if (candidate && this.connection?.state === signalR.HubConnectionState.Connected) this.connection.invoke('SendIceCandidate', this.sessionId, candidate.toJSON()).catch(() => {}) }
        this.peer.onconnectionstatechange = () => { const value = this.peer?.connectionState; this.state = value === 'connected' ? 'connected' : value === 'failed' ? 'failed' : value === 'disconnected' ? 'reconnecting' : 'connecting' }
        await this.connectSignalR()
      } catch (e) {
        this.error = e.name === 'NotAllowedError' ? 'Camera or microphone permission was denied. Enable permission in the browser and try again.' : (e.message || 'Unable to join the consultation.')
        this.state = 'failed'; this.joined = false; await this.cleanup()
      } finally { this.busy = false }
    },
    async connectSignalR() {
      this.connection = new signalR.HubConnectionBuilder().withUrl(HUB_URL, { accessTokenFactory: () => localStorage.getItem('access-token') || '' }).withAutomaticReconnect([0, 2000, 5000, 10000]).build()
      this.connection.on('ParticipantJoined', async () => { if (this.session.callerIsPatient) await this.createOffer() })
      this.connection.on('ReceiveOffer', async (_, offer) => { await this.peer.setRemoteDescription(offer); await this.flushCandidates(); const answer = await this.peer.createAnswer(); await this.peer.setLocalDescription(answer); await this.connection.invoke('SendAnswer', this.sessionId, { type: answer.type, sdp: answer.sdp }) })
      this.connection.on('ReceiveAnswer', async (_, answer) => { if (!this.peer.currentRemoteDescription) { await this.peer.setRemoteDescription(answer); await this.flushCandidates() } })
      this.connection.on('ReceiveIceCandidate', async (_, candidate) => { if (this.peer.remoteDescription) await this.peer.addIceCandidate(candidate); else this.pendingCandidates.push(candidate) })
      this.connection.on('ParticipantLeft', () => { if (!this.ended) this.state = 'reconnecting' })
      this.connection.on('ConsultationEnded', reason => { this.error = reason; this.ended = true; this.state = 'ended'; this.cleanup() })
      this.connection.onreconnecting(() => { this.state = 'reconnecting' })
      this.connection.onreconnected(async () => { await this.connection.invoke('JoinConsultation', this.sessionId); this.state = 'connecting' })
      this.connection.onclose(() => { if (!this.ended) this.state = 'failed' })
      await this.connection.start(); await this.connection.invoke('JoinConsultation', this.sessionId)
    },
    async createOffer() { if (!this.peer || this.peer.signalingState !== 'stable') return; const offer = await this.peer.createOffer({ iceRestart: this.state === 'reconnecting' }); await this.peer.setLocalDescription(offer); await this.connection.invoke('SendOffer', this.sessionId, { type: offer.type, sdp: offer.sdp }) },
    async flushCandidates() { for (const candidate of this.pendingCandidates.splice(0)) await this.peer.addIceCandidate(candidate) },
    toggleMicrophone() { const track = this.localStream?.getAudioTracks()[0]; if (track) { track.enabled = !track.enabled; this.muted = !track.enabled } },
    toggleCamera() { const track = this.localStream?.getVideoTracks()[0]; if (track) { track.enabled = !track.enabled; this.cameraEnabled = track.enabled } },
    async leave(end) { this.ended = end; try { if (end && this.connection?.state === signalR.HubConnectionState.Connected) await this.connection.invoke('EndConsultation', this.sessionId, 'Ended by participant'); else if (end) await endConsultation(this.sessionId, 'Ended by participant'); else if (this.connection?.state === signalR.HubConnectionState.Connected) await this.connection.invoke('LeaveConsultation', this.sessionId) } catch (_) {} await this.cleanup(); this.state = end ? 'ended' : 'ready'; this.joined = false; if (!end) this.$router.back() },
    async cleanup() { this.localStream?.getTracks().forEach(track => track.stop()); this.localStream = null; this.peer?.close(); this.peer = null; this.pendingCandidates = []; this.remoteMedia = false; if (this.connection) { try { await this.connection.stop() } catch (_) {} } this.connection = null },
  },
}
</script>
