/* eslint-disable */
import Vue from "vue";
// TODO: FIXME: temp https://github.com/intlify/vue-i18n/issues/1923
declare module "@vue/runtime-core" {
	export interface ComponentCustomProperties {
		$t: (key: string, ...args: any[]) => string;
		$n: (key: number, ...args: any[]) => string;
		$d: (key: Date, ...args: any[]) => string;
	}
}
