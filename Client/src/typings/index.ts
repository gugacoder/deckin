export class Dictionary<T> {
  [key: string]: T
}

export class Ref {
  name: string
  args: object
  constructor(name: string)
  constructor(name: string, args: object = {}) {
    this.name = name
    this.args = args
  }
}

// eslint-disable-next-line
export const isEntity = (obj: any): obj is Entity => {
  return obj?.type;
}

// eslint-disable-next-line
export const isRef = (obj: any): obj is Ref => {
  return obj?.name && obj?.args;
}

export type Entity =
  Unknown |
  Data |
  DataSet |
  Blueprint |
  Paper

export abstract class AbstractEntity {
  abstract type: string
  self?: Ref | string
  ref?: Ref | string
}

export class Unknown extends AbstractEntity {
  type: string
  self?: Ref | string
  constructor(type: string) {
    super()
    this.type = type
  }
}

export class Data extends AbstractEntity {
  readonly type = 'data'
  properties?: object
  subset?: Data[]
}

export class DataSet extends AbstractEntity {
  readonly type = 'dataSet'
  subset?: Data[]
}

/* #region Blueprints Variations */

type Blueprint =
  SplashBlueprint |
  CardBlueprint |
  EditBlueprint |
  GridBlueprint |
  ListBlueprint

export class SplashBlueprint extends AbstractEntity {
  readonly type = 'splash'
  title?: string
}

export class CardBlueprint extends AbstractEntity {
  readonly type = 'card'
}

export class EditBlueprint extends AbstractEntity {
  readonly type = 'edit'
}

export class GridBlueprint extends AbstractEntity {
  readonly type = 'grid'
}

export class ListBlueprint extends AbstractEntity {
  readonly type = 'list'
}

/* #endregion */

export class Paper extends AbstractEntity {
  readonly type = 'paper'
  dataSet?: DataSet
  blueprint?: Blueprint
}

export class RootState {
  entityList: Dictionary<Entity>
  entityKeys: string[]
  constructor() {
    this.entityList = new Dictionary<Entity>()
    this.entityKeys = []
  }
}

/* #region Helper Functions */

export function refToString(ref: Ref): string {
  // eslint-disable-next-line
  const args = ref.args as any
  const keys = Object.keys(args)
  const entries = keys.length
    ? keys.map(key => key ? `${key}=${args[key]}` : '').join(';')
    : ''
  return `${ref.name}(${entries})`.replace('()', '')
}

export function stringToRef(value: string): Ref {
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

export interface Visitor {
  (e: Entity): void;
}

export function visit<T extends Entity>(entity: T, visitor: Visitor): T {
  visitor(entity)
  Object.keys(entity).forEach(key => {
    // eslint-disable-next-line
    const value = (entity as any)[key]
    if (isEntity(value)) {
      visit(value, visitor)
    } else if (Array.isArray(value)) {
      value.forEach(child => {
        if (isEntity(child)) {
          visit(child, visitor)
        }
      })
    }
  })
  return entity
}

export function normalize<T extends Entity>(entity: T): T {
  return visit(entity, e => {
    if (typeof e.self === 'string') {
      e.self = stringToRef(e.self)
    }
    if (typeof e.ref === 'string') {
      e.ref = stringToRef(e.ref)
    }
  })
}

export function denormalize<T extends Entity>(entity: T): T {
  return visit(entity, e => {
    if (isRef(e.self)) {
      e.self = refToString(e.self)
    }
    if (isRef(e.ref)) {
      e.ref = refToString(e.ref)
    }
  })
}

/* #endregion */