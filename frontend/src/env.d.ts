/// <reference types="vite/client" />

import 'vue-router'

declare module 'vue-router' {
  interface RouteMeta {
    hideBack?: boolean
    backLabelKey?: string
    backTo?: import('vue-router').RouteLocationRaw
      | ((route: import('vue-router').RouteLocationNormalizedLoaded) => import('vue-router').RouteLocationRaw)
      | 'history'
  }
}

declare module '*.vue' {
  import type { DefineComponent } from 'vue'
  const component: DefineComponent<object, object, unknown>
  export default component
}
