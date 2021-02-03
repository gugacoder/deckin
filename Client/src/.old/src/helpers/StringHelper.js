function makeFirstLetterUpperCase (word) {
  return (word === '') ? '': (word[0].toUpperCase() + word.slice(1))
}

export function removeDiacritics(text) {
  return text.normalize("NFD").replace(/[\u0300-\u036f]/g, "")
}

export function wordify(text) {
  return text.
    split('').
    map(char =>
      char.match(/[A-ZÀ-Ö]/)
        ? ` ${char.toLowerCase()}`
        : char.match(/[^A-Za-zÀ-ÖØ-öø-ÿ0-9]/)
          ? ' '
          : char).
    join('').
    split(' ').
    filter(char => char !== '')
}

export function changeCase(text, textCase) {
  let words = wordify(text, textCase)
  switch (textCase) {
    case 'TitleCase':
    case 'ProperCase': {
      return words.map(makeFirstLetterUpperCase).join(' ')
    }
    case 'SentenceCase':
    case 'StartCase': {
      let first = makeFirstLetterUpperCase(words.shift())
      return [ first, ...words ].join(' ')
    }
    case 'CamelCase': {
      let first = words.shift()
      let others = words.map(makeFirstLetterUpperCase)
      return removeDiacritics([ first, ...others ].join(''))
    }
    case 'PascalCase': {
      words = words.map(makeFirstLetterUpperCase)
      return removeDiacritics(words.join(''))
    }
    case 'DotCase': {
      return removeDiacritics(words.join('.'))
    }
    case 'Hyphenated':
    case 'Dasherized':
    case 'DashCase':
    case 'HyphenCase': {
      return removeDiacritics(words.join('-'))
    }
    case 'Underscore':
    case 'SnakeCase': {
      return removeDiacritics(words.join('_'))
    }
    case 'AllCaps': {
      words = words.map(word => word.toUpperCase())
      return removeDiacritics(words.join('_'))
    }
    default: {
      return text
    }
  }
}

export default {
  wordify,
  changeCase
}

String.prototype.wordify = function () {
  return wordify(this)
}
String.prototype.changeCase = function (textCase) {
  return changeCase(this, textCase)
}
String.prototype.toProperCase = function () {
  return changeCase(this, 'ProperCase')
}
String.prototype.toTitleCase = function () {
  return changeCase(this, 'TitleCase')
}
String.prototype.toSentenceCase = function () {
  return changeCase(this, 'SentenceCase')
}
String.prototype.toStartCase = function () {
  return changeCase(this, 'StartCase')
}
String.prototype.toCamelCase = function () {
  return changeCase(this, 'CamelCase')
}
String.prototype.toPascalCase = function () {
  return changeCase(this, 'PascalCase')
}
String.prototype.toDotCase = function () {
  return changeCase(this, 'DotCase')
}
String.prototype.toHyphenCase = function () {
  return changeCase(this, 'HyphenCase')
}
String.prototype.toSnakeCase = function () {
  return changeCase(this, 'SnakeCase')
}
String.prototype.toAllCaps = function () {
  return changeCase(this, 'AllCaps')
}