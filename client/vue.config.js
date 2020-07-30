module.exports = {
  transpileDependencies: [
    'vuetify'
  ],
  devServer: {
    //host: '0.0.0.0',
    //port: 80,
    proxy: {
      "/Api/*": {
        target: 'http://localhost:5000',
        secure: false,
        changeOrigin: true
      }
    }
  }
}