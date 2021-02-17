import { Paper, normalize } from '@/typings'

const paper: Paper = {
  type: 'paper',
  self: 'Splash',
  blueprint: {
    type: 'splash'
  }
}

export const splash: Readonly<Paper> = normalize(paper)
