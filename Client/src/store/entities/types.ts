export interface Entity {
  self: string;
}

export interface Paper extends Entity {
}

export interface Data extends Entity {
  props: object
}
