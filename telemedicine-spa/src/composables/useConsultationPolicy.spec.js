import { describe, expect, it } from 'vitest'
import { chatAccessFor, videoAccessFor } from './useConsultationPolicy'

const future = new Date(Date.now() + 60 * 60 * 1000).toISOString()
const end = new Date(Date.now() + 90 * 60 * 1000).toISOString()

function appointment(status, overrides = {}) {
  return {
    id: 1,
    status,
    appointmentDate: future,
    scheduleEndTime: end,
    ...overrides,
  }
}

const strictPolicy = {
  allowOutsideWindow: false,
  joinWindowMinutesBefore: 15,
  joinWindowMinutesAfter: 30,
}

describe('consultation action state', () => {
  it('shows video and chat for an active appointment inside the join window', () => {
    const start = new Date(Date.now() + 5 * 60 * 1000).toISOString()
    const finish = new Date(Date.now() + 35 * 60 * 1000).toISOString()
    const item = appointment(0, { appointmentDate: start, scheduleEndTime: finish })

    expect(videoAccessFor(item, strictPolicy).canJoin).toBe(true)
    expect(chatAccessFor(item).canChat).toBe(true)
  })

  it('waits for an appointment outside the window unless demo bypass is enabled', () => {
    const item = appointment(0)

    expect(videoAccessFor(item, strictPolicy).state).toBe('waiting')
    expect(videoAccessFor(item, { ...strictPolicy, allowOutsideWindow: true }).canJoin).toBe(true)
  })

  it('blocks chat while pending and video after completion', () => {
    expect(chatAccessFor(appointment(4)).canChat).toBe(false)
    expect(videoAccessFor(appointment(4), strictPolicy).state).toBe('pending')
    expect(videoAccessFor(appointment(2), strictPolicy).state).toBe('ended')
  })
})
