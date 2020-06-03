module.exports = {
  "transpileDependencies": [
    "vuetify"
  ],
  "devServer": {
    "proxy": {
      "/Api/*": {
        "target": "http://localhost:5000",
        "secure": false
      },
      "/!/*": {
        "target": "http://localhost:5000",
        "secure": false
      }
    }
  }
}