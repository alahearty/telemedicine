import { mount, flushPromises } from '@vue/test-utils'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import Consultation from './Consultation.vue'

vi.mock('@/api', () => ({
  getConsultation: vi.fn(async () => ({ sessionId: 'session', status: 'Scheduled', callerIsPatient: true, patientName: 'Test Patient', physicianName: 'Test Physician' })),
  joinConsultation: vi.fn(), endConsultation: vi.fn(), getConsultationIceServers: vi.fn(async () => []),
}))

vi.mock('@microsoft/signalr', () => ({ HubConnectionState: { Connected: 'Connected' }, HubConnectionBuilder: vi.fn() }))

const mountView = () => mount(Consultation, {
  global: { mocks: { $route: { params: { sessionId: 'session' } }, $router: { back: vi.fn() } } },
})

describe('Consultation media lifecycle', () => {
  beforeEach(() => vi.restoreAllMocks())

  it('reports denied media permission without claiming a connection', async () => {
    Object.defineProperty(navigator, 'mediaDevices', { configurable: true, value: { getUserMedia: vi.fn().mockRejectedValue(Object.assign(new Error('denied'), { name: 'NotAllowedError' })) } })
    const wrapper = mountView(); await flushPromises(); await wrapper.vm.join()
    expect(wrapper.vm.state).toBe('failed')
    expect(wrapper.vm.joined).toBe(false)
    expect(wrapper.text()).toContain('permission was denied')
  })

  it('toggles tracks and stops every resource during cleanup', async () => {
    const wrapper = mountView(); await flushPromises()
    const audio = { enabled: true, stop: vi.fn() }; const video = { enabled: true, stop: vi.fn() }
    const close = vi.fn(); const stop = vi.fn(async () => {})
    wrapper.vm.localStream = { getTracks: () => [audio, video], getAudioTracks: () => [audio], getVideoTracks: () => [video] }
    wrapper.vm.peer = { close }; wrapper.vm.connection = { stop }
    wrapper.vm.toggleMicrophone(); wrapper.vm.toggleCamera()
    expect(audio.enabled).toBe(false); expect(video.enabled).toBe(false)
    await wrapper.vm.cleanup()
    expect(audio.stop).toHaveBeenCalledOnce(); expect(video.stop).toHaveBeenCalledOnce()
    expect(close).toHaveBeenCalledOnce(); expect(stop).toHaveBeenCalledOnce()
  })
})
