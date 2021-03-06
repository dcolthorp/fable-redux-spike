"use strict"

var concat = require('broccoli-concat'),
    mergeTrees  = require('broccoli-merge-trees'),
    funnel   = require('broccoli-funnel'),
    esTranspiler = require('broccoli-babel-transpiler');

var pkg = 'web';

var appHtml = funnel('src', {
  srcDir  : '/',
  files   : ['index.html'],
  destDir : '/'
});

// var maps = funnel('out', {
//   include: ["**/*.map"],
//   destDir : 'js'
// });

var transpiled = esTranspiler('out', {
  stage: 0,
  moduleIds: true,
  modules: 'amd',
  sourceMaps: true,
  only: '**/*.js',
});

var publics = funnel('public', {
  include   : ['**/*'],
  destDir: '.',
});

// var fableScripts = funnel(transpiled, {
//   include   : ['**/*.js'],
//   destDir: 'out',
// });

// var main = concat(fableScripts, {
//   inputFiles: "**/*.js",
//   outputFile: '/js/web.js'
// });

// var nodeVendorJs = funnel('node_modules', {
//   srcDir  : '/',
//   include   : ['core-js/client/*',
//              'requirejs/*',
//              'fable-core/*'
//             ],
//   destDir: '/js'
// });


module.exports = mergeTrees([
  appHtml,
  publics
  // main,
  // nodeVendorJs,
  // bower //,
  // maps
]);
