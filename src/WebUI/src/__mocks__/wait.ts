export const mockStartMethod = vi.fn()
export const mockEndMethod = vi.fn()
export const mockIsMethod = vi.fn()
export const mockAnyMethod = vi.fn()

const mock = vi.fn().mockImplementation(() => ({
  any: mockAnyMethod,
  end: mockEndMethod,
  is: mockIsMethod,
  start: mockStartMethod,
}))

export default mock
