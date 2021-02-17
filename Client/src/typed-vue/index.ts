import Vue, { VueConstructor } from 'vue'
import { Store } from 'vuex'
import { RootState } from '@/typings'

abstract class AbstractStrongVue extends Vue {
  public $store!: Store<RootState>;
}
const TypedVue = Vue as VueConstructor<AbstractStrongVue>;

export default TypedVue;