import { refToString, stringToRef } from './helpers'

export class Dictionary<T> {
  [key: string]: T
}

export class Ref {
  name: string
  args: object
  get path(): string { return refToString(this) }

  constructor()
  constructor(pathOrName: string)
  constructor(pathOrName = '', args: object = {}) {
    if (pathOrName.includes('(')) {
      const parsed = stringToRef(pathOrName)
      this.name = parsed.name
      this.args = { ...parsed.args, ...args }
    } else {
      this.name = pathOrName
      this.args = args
    }
  }

  static parsePath(path: string): Ref {
    const ref: Ref = new Ref()
    const parsed = stringToRef(path)
    ref.name = parsed.name
    ref.args = parsed.args
    return ref
  }
}

export class EntityRef {
  type: string | undefined
  ref: Ref | undefined
}

export abstract class Entity {
  abstract type: string
  self: Ref | undefined
}

export class Data extends Entity {
  type = 'data' 
  properties: object | undefined
  subset: EntityRef[] | Entity[] | undefined
}

export class DataSet extends Entity {
  type = 'dataSet' 
  set: Entity[] | EntityRef[] = []
}

export abstract class Disposition extends Entity {
  type = 'disposition' 
  abstract blueprint: string | undefined
}

/* #region Disposition Variations */

export class SplashDisposition extends Disposition {
  blueprint = 'splash'
}

export class CardDisposition extends Disposition {
  blueprint = 'card'
}

export class EditDisposition extends Disposition {
  blueprint = 'edit'
}

export class GridDisposition extends Disposition {
  blueprint = 'grid'
}

export class ListDisposition extends Disposition {
  blueprint = 'list'
}

/* #endregion */

export class Paper extends Entity {
  type = 'paper' 
  data: DataSet | undefined
  disposition: Disposition | undefined
}

export class State {
  dataList: Dictionary<Data>
  dataKeys: string[]

  paperList: Dictionary<Paper>
  paperKeys: string[]

  constructor() {
    this.dataList = new Dictionary<Data>()
    this.dataKeys = []
    this.paperList = new Dictionary<Paper>()
    this.paperKeys = []
  }
}