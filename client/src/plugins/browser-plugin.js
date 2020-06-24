export default {
  install (Vue, options) {
    Vue.mixin({
      created() {
        console.log('ready...', options)
      }
    })
  }
}