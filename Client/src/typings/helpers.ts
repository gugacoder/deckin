export interface ParsedRef {
  name: string;
  args: object;
}

export function refToString(ref: ParsedRef): string {
  // eslint-disable-next-line
  const args = ref.args as any
  const keys = Object.keys(args)
  const entries = keys.length
    ? keys.map(key => key ? `${key}=${args[key]}` : '').join(';')
    : ''
  return `${ref.name}(${entries})`.replace('()', '')
}

export function stringToRef(value: string): ParsedRef {
  const tokens = value.replace(')', '').split('(')

  const name = tokens[0]

  // eslint-disable-next-line
  const args: any = {}

  const entries = tokens[1] ?? ''
  entries.split(';').map(entry => {
    const tokens = entry.split('=')
    const key = tokens[0]
    const value = tokens.slice(1).join('=')
    args[key] = value
  })

  return { name, args }
}
