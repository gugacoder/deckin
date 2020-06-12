import Vue from 'vue'

//
// Importação global de todos os componentes *Paper.vue.
//

const componentCodes = require.context(
  // Pasta de pesquisa...
  '.',
  // Varrer sub-pastas?
  false,
  // Padrão de nome dos arquivos de componentes: *Widget.vue
  /[\w-]+Widget\.vue$/
)

componentCodes.keys().forEach((filename) => {
  const componentCode = componentCodes(filename)
  const componentName = filename
    // Removendo o "./" inicial...
    .replace(/^\.\//, '')
    // Removendo a extensão ".vue"...
    .replace(/\.\w+$/, '')

  // Registrando o componente globalmente...
  Vue.component(componentName, componentCode.default || componentCode)
})
