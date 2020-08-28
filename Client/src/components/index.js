import Vue from 'vue'

//
// Importação global de todos os componentes *Paper.vue.
//

const filepaths = require.context(
  // Pasta de pesquisa...
  '.',
  // Varrer sub-pastas?
  true,
  // Padrão de nome dos arquivos de componentes: *Paper.vue e *Widget.vue
  /[\w-]+(Paper|Slice|Widget)\.vue$/
)

filepaths.keys().forEach((filepath) => {
  const sourceCode = filepaths(filepath)
  const targetName = filepath.split('/')
    // Pegando o nome do arquivo
    .slice(-1).pop()
    // Removendo a extensão
    .replace(/\.\w+$/, '')
    // Hifenizando "foo-bar"
    .toHyphenCase()
  
  // Registrando o componente globalmente...
  Vue.component(targetName, sourceCode.default || sourceCode)
})
