<template lang="pug">
  v-footer.the-footer(
    app
  )
    slot(
      name="left"
    )

    v-spacer
    
    slot

    slot(
      name="right"
    )

</template>

<script>
export default {
  name: 'the-footer',
}
</script>