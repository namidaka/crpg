<script setup lang="ts">
import { ref } from 'vue';

enum HelpTopics {
    Overview = 'Overview',
    BuildSupport = 'BuildSupport',
    JoinClan = 'JoinClan',
    FAQ = 'FAQ'
}

const tabsModel = ref<HelpTopics>(HelpTopics.Overview);
</script>

<template>
    <Modal closable>
        <slot>
            <OButton
            variant="secondary"
            size="xl"
            iconLeft="help-circle"
            :label="$t('help.title')"
            />
        </slot>

        <template #popper>
            <div class="prose prose-invert space-y-6 px-12 py-10">
                <h1 class="text-center">{{ $t('help.title') }}</h1>

                <OTabs v-model="tabsModel" size="xl" :animation="['fade', 'fade']">
                    <OTabItem :value="HelpTopics.Overview">
                        <template #header>{{ $t('help.tabs.overview') }}</template>
                        <h3 class="text-center">{{  $t('help.contentoverview.title2') }}</h3>
                        <template v-for="(line, index) in $t('help.contentoverview.welcomeText').split('\n\n')" :key="`line-${index}`">
                            <p v-if="line.trim() !== ''">{{ line }}</p>
                        </template>
                    </OTabItem>

                    <OTabItem :value="HelpTopics.BuildSupport">
                        <template #header>{{ $t('help.tabs.buildSupport') }}</template>
                        <h3 class="text-center">{{  $t('help.contentbuildSupport.title3') }}</h3>
                        <template v-for="(line, index) in $t('help.contentbuildSupport.buildSupport').split('\n\n')" :key="`line-${index}`">
                            <p v-if="line.trim() !== ''">{{ line }}</p>
                        </template>
                    </OTabItem>

                    <OTabItem :value="HelpTopics.JoinClan">
                        <template #header>{{ $t('help.tabs.joinClan') }}</template>
                        <h3 class="text-center">{{  $t('help.contentjoinClan.title4') }}</h3>
                        <template v-for="(line, index) in $t('help.contentjoinClan.joinClan').split('\n\n')" :key="`line-${index}`">
                            <p v-if="line.trim() !== ''">{{ line }}</p>
                        </template>
                    </OTabItem>

                    <OTabItem :value="HelpTopics.FAQ">
                        <template #header>{{ $t('help.tabs.faq') }}</template>
                        <h3 class="text-center">{{  $t('help.contentfaq.title5') }}</h3>
                        <p>{{ $t('help.contentfaq.faq') }}</p>
                        <h5 class="text-center">{{  $t('help.contentfaq.q1') }}</h5>
                        <p>{{ $t('help.contentfaq.a1') }}</p>
                        <h5 class="text-center">{{  $t('help.contentfaq.q2') }}</h5>
                        <p>{{ $t('help.contentfaq.a2') }}</p>
                        <h5 class="text-center">{{  $t('help.contentfaq.q3') }}</h5>
                        <p>{{ $t('help.contentfaq.a3') }}</p>
                        <h5 class="text-center">{{  $t('help.contentfaq.q4') }}</h5>
                        <p>{{ $t('help.contentfaq.a4') }}</p>
                        <h5 class="text-center">{{  $t('help.contentfaq.q5') }}</h5>
                        <p>{{ $t('help.contentfaq.a5') }}</p>
                    </OTabItem>

                    <div class="mt-6 space-y-6">
          <i18n-t scope="global" keypath="discordfooter.discord" tag="p" class="text-content-100">
            <template #discordLink>
              <!-- prettier-ignore -->
              <a
                target="_blank"
                href="https://discord.gg/c-rpg"
              >Discord</a>
            </template>
          </i18n-t>
        </div>
                </OTabs>
            </div>
        </template>
    </Modal>
</template>
