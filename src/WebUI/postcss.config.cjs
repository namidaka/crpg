// @ts-check

/** @type {import('postcss-load-config').Config} */
const config = {
  plugins: {
    autoprefixer: {},
    tailwindcss: {},
    ...(require('node:process').env.NODE_ENV === 'production' ? { cssnano: {} } : {}),
  },
}

module.exports = config
