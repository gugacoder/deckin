import { Paper, normalize } from '@/typings'

const paper: Readonly<Paper> = {
  type: 'paper',
  self: 'Home',
  blueprint: {
    type: 'card'
  }
}

export const home: Readonly<Paper> = normalize(paper)
