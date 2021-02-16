import { State, Paper, Ref, ListDisposition, SplashDisposition } from '../typings'

const state = new State()

const splash: Paper = new Paper()
splash.self = new Ref('Splash')
splash.disposition = new SplashDisposition()
state.paperKeys.push(splash.self.path)
state.paperList[splash.self.path] = splash

const home: Paper = new Paper()
home.self = new Ref('Home')
home.disposition = new ListDisposition()
state.paperKeys.push(home.self.path)
state.paperList[home.self.path] = home

export const InitialState = state