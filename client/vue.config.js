module.exports = {
  transpileDependencies: [
    'vuetify'
  ],
  devServer: {
    //host: '0.0.0.0',
    //port: 80,
    //proxy: {
    //  "/Api/*": {
    //    target: 'http://localhost:5000',
    //    secure: false,
    //    changeOrigin: true
    //  }
    //},
  },

  pwa: {
    name: 'ProcessaApp',
    themeColor: '#673ab7',
    msTileColor: '#673ab7',
    appleMobileWebAppCapable: 'yes',
    appleMobileWebAppStatusBarStyle: 'black',

    workboxPluginMode: 'InjectManifest',
    workboxOptions: {
      swSrc: 'src/service-worker.js',
    }
  }
}