//
// Importação automática dos módulos do Vuex
//

const filepaths = require.context(
  // Pasta de pesquisa...
  '.',
  // Varrer sub-pastas?
  true,
  // Padrão de nome dos arquivos de componentes: *Paper.vue e *Widget.vue
  /[\w-]+Store\.vue$/
)

const modules = {}

filepaths.keys().forEach((filepath) => {
  const sourceCode = filepaths(filepath)
  const targetName = filepath.split('/')
    // Pegando o nome do arquivo
    .slice(-1).pop()
    // Removendo o sufixo Store.js
    .replace(/Store\.\w+$/, '')
    // Camelizando "fooBar"
    .toCamelCase()
    
  // Adicionando o módulo à coleção de módulos
  Object.assign(modules, targetName, sourceCode.default)
})

export default modules
