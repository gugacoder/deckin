import Vue from 'vue'

//
// Importação global de todos os componentes *Paper.vue.
//

const componentCodes = require.context(
  // Pasta de pesquisa...
  '.',
  // Varrer sub-pastas?
  true,
  // Padrão de nome dos arquivos de componentes: *Paper.vue e *Widget.vue
  /[\w-]+(Paper|Widget)\.vue$/
)

componentCodes.keys().forEach((filepath) => {
  const componentCode = componentCodes(filepath)
  const componentName = filepath.split('/')
    // Pegando o nome do arquivo
    .slice(-1).pop()
    // Removendo a extensão
    .replace(/\.\w+$/, '')
    // Hifenizando "foo-bar"
    .toHyphenCase()
    
  // Registrando o componente globalmente...
  Vue.component(componentName, componentCode.default || componentCode)
})
