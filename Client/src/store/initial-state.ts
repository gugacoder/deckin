import { Ref, RootState, refToString } from '@/typings'
import { papers } from '@/papers'

const state = new RootState()

papers.forEach(paper => {
  state.entityKeys.push(refToString(paper.self as Ref))
  state.entityList[refToString(paper.self as Ref)] = paper
});

export const initialState = state
