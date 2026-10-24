<template>
  <div class="space-y-5">
    <div class="flex flex-wrap items-end justify-between gap-3">
      <div>
        <p class="eyebrow">Care messages</p>
        <h1 class="page-title mt-1">Messages</h1>
        <p class="page-subtitle">Secure conversations for your accepted appointments.</p>
      </div>
      <span class="badge badge-neutral">
        <Icon icon="message-circle" class="text-[11px]" />
        {{ threads.length }} {{ threads.length === 1 ? 'conversation' : 'conversations' }}
      </span>
    </div>

    <div class="card flex h-[calc(100dvh-250px)] min-h-[540px] overflow-hidden">
      <aside
        class="w-full shrink-0 flex-col border-r border-slate-100 dark:border-white/[0.06] lg:flex lg:w-[340px]"
        :class="selectedThread ? 'hidden' : 'flex'"
      >
        <div class="border-b border-slate-100 p-4 dark:border-white/[0.06]">
          <div class="relative">
            <Icon icon="search" class="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-sm text-slate-400" />
            <input
              v-model="search"
              type="search"
              class="input-modern py-2 pl-9"
              placeholder="Search conversations..."
              aria-label="Search conversations"
            />
          </div>
        </div>

        <div v-if="loadingThreads" class="space-y-3 p-4">
          <div v-for="n in 4" :key="n" class="flex items-center gap-3">
            <div class="h-10 w-10 rounded-full skeleton" />
            <div class="flex-1 space-y-2"><div class="h-3 w-1/2 skeleton" /><div class="h-2.5 w-3/4 skeleton" /></div>
          </div>
        </div>

        <div v-else-if="filteredThreads.length" class="scrollbar-slim flex-1 overflow-y-auto p-2">
          <button
            v-for="thread in filteredThreads"
            :key="thread.appointmentId"
            type="button"
            class="flex w-full items-start gap-3 rounded-xl p-3 text-left transition-colors"
            :class="selectedThread?.appointmentId === thread.appointmentId
              ? 'bg-primary-50/80 ring-1 ring-primary-100 dark:bg-primary-400/[0.08] dark:ring-primary-400/15'
              : 'hover:bg-slate-50 dark:hover:bg-white/[0.03]'"
            @click="selectThread(thread)"
          >
            <AppAvatar :name="thread.counterpartName" size="md" />
            <span class="min-w-0 flex-1">
              <span class="flex items-center justify-between gap-2">
                <span class="truncate text-xs font-semibold text-ink-900 dark:text-slate-100">{{ thread.counterpartName }}</span>
                <span class="shrink-0 text-[10px] text-slate-400 dark:text-slate-500">{{ threadTime(thread) }}</span>
              </span>
              <span class="mt-0.5 block truncate text-[11px] text-slate-500 dark:text-slate-400">
                {{ thread.serviceName }} &middot; {{ formatDateShort(thread.appointmentDate) }}
              </span>
              <span class="mt-1 flex items-center gap-2">
                <span class="min-w-0 flex-1 truncate text-[11px]" :class="thread.unreadCount ? 'font-semibold text-slate-700 dark:text-slate-200' : 'text-slate-400 dark:text-slate-500'">
                  {{ thread.lastMessage || 'No messages yet' }}
                </span>
                <span v-if="thread.unreadCount" class="grid h-5 min-w-5 place-items-center rounded-full bg-primary-500 px-1.5 text-[10px] font-bold text-white">
                  {{ thread.unreadCount > 9 ? '9+' : thread.unreadCount }}
                </span>
              </span>
            </span>
          </button>
        </div>

        <EmptyState
          v-else-if="!threads.length"
          class="flex-1"
          icon="message-circle"
          title="No conversations yet"
          description="Accepted appointments create a secure chat thread with your physician."
        >
          <router-link to="/dashboard/appointments" class="btn-secondary">View appointments</router-link>
        </EmptyState>
        <p v-else class="p-8 text-center text-xs text-slate-500 dark:text-slate-400">No conversations match your search.</p>
      </aside>

      <section class="min-w-0 flex-1 flex-col" :class="selectedThread ? 'flex' : 'hidden lg:flex'">
        <template v-if="selectedThread">
          <header class="flex flex-wrap items-center gap-3 border-b border-slate-100 p-4 dark:border-white/[0.06] sm:p-5">
            <div class="flex min-w-0 flex-1 items-center gap-3">
              <button type="button" class="btn-icon h-9 w-9 shrink-0 lg:hidden" aria-label="Back to conversations" @click="selectedThread = null">
                <Icon icon="arrow-left" class="text-base" />
              </button>
              <AppAvatar :name="selectedThread.counterpartName" size="md" />
              <div class="min-w-0 flex-1">
                <p class="truncate text-sm font-semibold text-ink-900 dark:text-slate-100">{{ selectedThread.counterpartName }}</p>
                <p class="mt-0.5 truncate text-[11px] text-slate-500 dark:text-slate-400">
                  {{ selectedThread.serviceName }} &middot; {{ formatDateShort(selectedThread.appointmentDate) }}
                </p>
              </div>
            </div>
            <div class="flex w-full flex-wrap items-center justify-end gap-2 sm:w-auto">
              <StatusBadge :status="selectedThread.status" />
              <button
                v-if="video.canJoin"
                type="button"
                class="btn-primary px-3 py-2 text-xs"
                :disabled="joiningVideo"
                @click="joinVideo"
              >
                <Icon icon="video" class="text-sm" />
                {{ joiningVideo ? 'Opening...' : 'Join video' }}
              </button>
              <span v-else-if="video.state === 'waiting' || video.state === 'closed'" class="text-[10px] font-medium text-slate-400 dark:text-slate-500">
                {{ video.detail }}
              </span>
              <router-link :to="detailsLink" class="btn-secondary hidden px-3 py-2 text-xs sm:inline-flex">Details</router-link>
            </div>
          </header>

          <div
            v-if="connectionState === 'reconnecting' || connectionState === 'offline'"
            class="border-b border-amber-200 bg-amber-50 px-4 py-2 text-[11px] font-medium text-amber-700 dark:border-amber-400/20 dark:bg-amber-400/10 dark:text-amber-300"
          >
            {{ connectionState === 'reconnecting' ? 'Reconnecting to live chat...' : 'Live connection unavailable. Messages will still be saved.' }}
          </div>

          <div ref="messagesContainer" class="scrollbar-slim flex-1 space-y-3 overflow-y-auto bg-slate-50/50 p-4 dark:bg-white/[0.01] sm:p-5">
            <div v-if="loadingMessages" class="space-y-3">
              <div v-for="n in 4" :key="n" class="flex" :class="n % 2 ? 'justify-start' : 'justify-end'">
                <div class="h-11 w-2/5 skeleton rounded-2xl" />
              </div>
            </div>

            <template v-else-if="messages.length">
              <template v-for="(message, index) in messages" :key="message.id || index">
                <div v-if="showDateDivider(index)" class="flex items-center gap-3 py-2">
                  <span class="h-px flex-1 bg-slate-200/70 dark:bg-white/[0.06]" />
                  <span class="text-[10px] font-semibold uppercase tracking-[0.1em] text-slate-400 dark:text-slate-500">{{ formatDateShort(message.sentAtUtc) }}</span>
                  <span class="h-px flex-1 bg-slate-200/70 dark:bg-white/[0.06]" />
                </div>
                <div class="flex" :class="message.isMine ? 'justify-end' : 'justify-start'">
                  <div class="max-w-[85%] sm:max-w-[72%]">
                    <div
                      class="whitespace-pre-wrap rounded-2xl px-4 py-2.5 text-sm leading-6"
                      :class="message.isMine
                        ? 'rounded-br-md bg-gradient-to-br from-teal-500 to-cyan-500 text-white shadow-sm'
                        : 'rounded-bl-md border border-slate-200/80 bg-white text-slate-700 shadow-sm dark:border-white/[0.08] dark:bg-midnight-850 dark:text-slate-200'"
                    >
                      {{ message.body }}
                    </div>
                    <p class="mt-1 text-[10px] text-slate-400 dark:text-slate-500" :class="message.isMine ? 'text-right' : ''">
                      {{ message.isMine ? 'You' : message.senderName }} &middot; {{ formatTime(message.sentAtUtc) }}
                    </p>
                  </div>
                </div>
              </template>
            </template>

            <EmptyState
              v-else
              icon="message-circle"
              title="Start the conversation"
              description="Send a message to your care team. Messages are saved with this appointment."
            />
          </div>

          <div class="border-t border-slate-100 p-3 dark:border-white/[0.06] sm:p-4">
            <div v-if="errorMessage" class="mb-2 rounded-lg bg-rose-50 px-3 py-2 text-[11px] text-rose-700 dark:bg-rose-400/10 dark:text-rose-300">
              {{ errorMessage }}
            </div>
            <div class="flex items-end gap-2">
              <textarea
                v-model="newMessage"
                rows="1"
                maxlength="4000"
                :disabled="!chat.canChat || sending"
                class="input-modern max-h-28 min-h-[44px] flex-1 resize-none py-2.5"
                :placeholder="chat.canChat ? 'Write a message...' : chat.detail"
                aria-label="Message"
                @keydown.enter.exact.prevent="send"
              />
              <button
                type="button"
                class="inline-flex h-11 w-11 shrink-0 items-center justify-center rounded-xl bg-gradient-to-br from-teal-500 to-cyan-500 text-white shadow-sm transition hover:from-teal-400 hover:to-cyan-400 disabled:cursor-not-allowed disabled:opacity-40"
                :disabled="!chat.canChat || !newMessage.trim() || sending"
                aria-label="Send message"
                @click="send"
              >
                <Icon icon="send" class="text-lg" />
              </button>
            </div>
            <p class="mt-1.5 text-[10px] text-slate-400 dark:text-slate-500">
              Messages are saved to this appointment. Do not share urgent medical emergencies here.
            </p>
          </div>
        </template>

        <div v-else class="flex flex-1 items-center justify-center p-8">
          <EmptyState
            icon="message-circle"
            title="Select a conversation"
            description="Choose an appointment thread on the left to view messages and consultation actions."
          />
        </div>
      </section>
    </div>
  </div>
</template>

<script>
import * as signalR from '@microsoft/signalr'
import AppAvatar from '@/components/ui/AppAvatar.vue'
import EmptyState from '@/components/ui/EmptyState.vue'
import StatusBadge from '@/components/ui/StatusBadge.vue'
import { createOrGetConsultation, getAppointmentMessages, getChatThreads, markAppointmentMessagesRead, sendAppointmentChatMessage } from '@/api'
import { chatAccessFor, ensureConsultationPolicy, useConsultationPolicy, videoAccessFor } from '@/composables/useConsultationPolicy'
import { formatDateShort, formatTime } from '@/utils/appointments'

export default {
  name: 'Chat',
  components: { AppAvatar, EmptyState, StatusBadge },
  setup() {
    const { state } = useConsultationPolicy()
    return { policyState: state }
  },
  data() {
    return {
      threads: [],
      messages: [],
      selectedThread: null,
      loadingThreads: true,
      loadingMessages: false,
      search: '',
      newMessage: '',
      sending: false,
      joiningVideo: false,
      errorMessage: '',
      connection: null,
      connectionState: 'connecting',
    }
  },
  computed: {
    isPhysician() {
      return this.$route.path.startsWith('/physician')
    },
    filteredThreads() {
      const term = this.search.trim().toLowerCase()
      if (!term) return this.threads
      return this.threads.filter((thread) =>
        `${thread.counterpartName} ${thread.serviceName} ${thread.lastMessage || ''}`.toLowerCase().includes(term)
      )
    },
    chat() {
      return chatAccessFor(this.selectedThread)
    },
    video() {
      return videoAccessFor(this.selectedThread, this.policyState.policy)
    },
    detailsLink() {
      if (!this.selectedThread) return this.isPhysician ? '/physician/appointments' : '/dashboard/appointments'
      return this.isPhysician
        ? `/physician/appointments/${this.selectedThread.appointmentId}`
        : `/dashboard/appointments/${this.selectedThread.appointmentId}`
    },
  },
  watch: {
    '$route.query.appointmentId'(value) {
      if (!value || !this.threads.length) return
      const id = Number(value)
      if (this.selectedThread?.appointmentId === id) return
      const thread = this.threads.find((item) => item.appointmentId === id)
      if (thread) this.selectThread(thread)
    },
  },
  async mounted() {
    ensureConsultationPolicy()
    await this.loadThreads()
    await this.connect()
  },
  beforeUnmount() {
    if (this.connection) {
      this.connection.stop()
      this.connection = null
    }
  },
  methods: {
    formatDateShort,
    formatTime,
    threadTime(thread) {
      const value = thread.lastMessageAtUtc || thread.appointmentDate
      if (!value) return ''
      const date = new Date(value)
      if (Number.isNaN(date.getTime())) return ''
      const today = new Date()
      if (date.toDateString() === today.toDateString()) return formatTime(value)
      return formatDateShort(value)
    },
    async loadThreads() {
      this.loadingThreads = true
      try {
        const data = await getChatThreads()
        this.threads = Array.isArray(data) ? data : (data?.data ?? [])
        const queryId = Number(this.$route.query.appointmentId)
        const target = this.threads.find((thread) => thread.appointmentId === queryId)
          || (window.matchMedia('(min-width: 1024px)').matches ? this.threads[0] : null)
        if (target) await this.selectThread(target, { updateRoute: Boolean(queryId) })
      } catch (error) {
        this.errorMessage = error.message || 'Could not load conversations.'
        if (this.$notification) this.$notification.error('Messages unavailable', this.errorMessage)
      } finally {
        this.loadingThreads = false
      }
    },
    async selectThread(thread, options = {}) {
      if (!thread) return
      this.selectedThread = thread
      this.errorMessage = ''
      this.newMessage = ''
      if (options.updateRoute !== false && String(this.$route.query.appointmentId || '') !== String(thread.appointmentId)) {
        this.$router.replace({ query: { ...this.$route.query, appointmentId: String(thread.appointmentId) } }).catch(() => {})
      }
      await this.loadMessages(thread)
    },
    async loadMessages(thread) {
      this.loadingMessages = true
      try {
        const data = await getAppointmentMessages(thread.appointmentId)
        this.messages = (Array.isArray(data) ? data : (data?.data ?? []))
          .slice()
          .sort((a, b) => new Date(a.sentAtUtc) - new Date(b.sentAtUtc))
        thread.unreadCount = 0
        markAppointmentMessagesRead(thread.appointmentId).then(() => window.dispatchEvent(new CustomEvent('telemedicine:chat-read'))).catch(() => {})
        this.$nextTick(this.scrollToBottom)
      } catch (error) {
        this.errorMessage = error.message || 'Could not load messages.'
      } finally {
        this.loadingMessages = false
      }
    },
    async connect() {
      if (this.connection) return
      this.connectionState = 'connecting'
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl('/chat-hub', { accessTokenFactory: () => localStorage.getItem('access-token') || '' })
        .withAutomaticReconnect([0, 2000, 5000, 10000])
        .configureLogging(signalR.LogLevel.Warning)
        .build()
      this.connection.on('ReceiveAppointmentMessage', this.handleIncomingMessage)
      this.connection.onreconnecting(() => { this.connectionState = 'reconnecting' })
      this.connection.onreconnected(() => { this.connectionState = 'connected'; this.loadThreads() })
      this.connection.onclose(() => { this.connectionState = 'offline' })
      try {
        await this.connection.start()
        this.connectionState = 'connected'
      } catch {
        this.connectionState = 'offline'
      }
    },
    handleIncomingMessage(message) {
      if (!message) return
      const role = (localStorage.getItem('user-role') || '').toLowerCase()
      const normalized = {
        ...message,
        isMine: String(message.senderRole || '').toLowerCase() === (role.includes('physician') || role.includes('doctor') ? 'physician' : 'patient'),
      }
      const thread = this.threads.find((item) => item.appointmentId === normalized.appointmentId)
      if (!thread) {
        this.loadThreads()
        return
      }
      thread.lastMessage = normalized.body
      thread.lastMessageAtUtc = normalized.sentAtUtc
      if (this.selectedThread?.appointmentId === normalized.appointmentId) {
        if (!this.messages.some((item) => item.id === normalized.id)) this.messages.push(normalized)
        thread.unreadCount = 0
        markAppointmentMessagesRead(normalized.appointmentId).then(() => window.dispatchEvent(new CustomEvent('telemedicine:chat-read'))).catch(() => {})
        this.$nextTick(this.scrollToBottom)
      } else if (!normalized.isMine) {
        thread.unreadCount = (thread.unreadCount || 0) + 1
      }
    },
    async send() {
      const text = this.newMessage.trim()
      if (!text || !this.selectedThread || !this.chat.canChat || this.sending) return
      this.sending = true
      this.errorMessage = ''
      try {
        if (this.connection && this.connection.state === signalR.HubConnectionState.Connected) {
          await this.connection.invoke('SendAppointmentMessage', this.selectedThread.appointmentId, text)
        } else {
          const result = await sendAppointmentChatMessage(this.selectedThread.appointmentId, text)
          const message = result?.message || result
          if (message && !this.messages.some((item) => item.id === message.id)) {
            this.messages.push(message)
            this.selectedThread.lastMessage = message.body
            this.selectedThread.lastMessageAtUtc = message.sentAtUtc
            this.$nextTick(this.scrollToBottom)
          }
        }
        this.newMessage = ''
      } catch (error) {
        this.errorMessage = error.message || 'Could not send the message.'
      } finally {
        this.sending = false
      }
    },
    async joinVideo() {
      if (!this.selectedThread || this.joiningVideo) return
      this.joiningVideo = true
      try {
        const session = await createOrGetConsultation(this.selectedThread.appointmentId)
        this.$router.push({
          name: this.isPhysician ? 'PhysicianConsultation' : 'PatientConsultation',
          params: { sessionId: session.sessionId },
        })
      } catch (error) {
        this.errorMessage = error.message || 'The video room is not available.'
        if (this.$notification) this.$notification.error('Video unavailable', this.errorMessage)
      } finally {
        this.joiningVideo = false
      }
    },
    showDateDivider(index) {
      if (index === 0) return true
      const current = new Date(this.messages[index]?.sentAtUtc).toDateString()
      const previous = new Date(this.messages[index - 1]?.sentAtUtc).toDateString()
      return current !== previous
    },
    scrollToBottom() {
      const container = this.$refs.messagesContainer
      if (container) container.scrollTop = container.scrollHeight
    },
  },
}
</script>

