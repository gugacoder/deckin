Storage.prototype.getObject = function(key) {
  let data = this.getItem(key)
  return data ? JSON.parse(data) : null
}

Storage.prototype.setObject = function(key, value) {
  let data = value ? JSON.stringify(value) : null
  this.setItem(key, data)
}

export default {}
