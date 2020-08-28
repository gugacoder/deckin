const AppConfig = require('./src/AppConfig.js')

module.exports = {
  transpileDependencies: [
    'vuetify'
  ],

  configureWebpack: {
    name: AppConfig.name
  },

  chainWebpack: config => {
    config
      .plugin('html')
      .tap(args => {
        args[0].AppConfig = AppConfig
        return args
      })
  },

  pwa: {
    name: AppConfig.title,
    themeColor: '#673ab7',
    msTileColor: '#673ab7',
    appleMobileWebAppCapable: 'yes',
    appleMobileWebAppStatusBarStyle: 'black',

    workboxPluginMode: 'InjectManifest',
    workboxOptions: {
      swSrc: 'src/service-worker.js',
    }
  },

  devServer: {
    //host: '0.0.0.0',
    //port: 80,
    proxy: {
     "/Api/*": {
       target: 'http://localhost:5050',
       secure: false,
       changeOrigin: true
     }
    },
  },
}