<script setup lang="ts"></script>

<template>
  <main>
    <RouterView v-slot="{ Component }">
      <Suspense>
        <component :is="Component" />
        <template #fallback>
          <OLoading
            full-page
            active
            icon-size="xl"
          />
        </template>
      </Suspense>
    </RouterView>
  </main>
</template>
