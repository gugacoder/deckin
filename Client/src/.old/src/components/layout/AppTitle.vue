<template lang="pug">
  span.app-title  
    span.font-weight-light {{ $app.banner.prefix }}
    span.font-weight-bold {{ $app.banner.title }}
    span.font-weight-light {{ $app.banner.suffix }}
      sup
        small.font-weight-light {{ $app.banner.variant }}
</template>
