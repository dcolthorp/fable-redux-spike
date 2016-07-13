var concat = require('broccoli-concat'),
    mergeTrees  = require('broccoli-merge-trees'),
    funnel   = require('broccoli-funnel'),
    esTranspiler = require('broccoli-babel-transpiler');

var pkg = 'web';

var appHtml = funnel('web', {
  srcDir  : '/',
  files   : ['index.html'],
  destDir : '/'
});

var transpiled = esTranspiler('out', {
  stage: 0,
  moduleIds: true,
  modules: 'amd',
  sourceMaps: false,
  only: '**/*.js',
});

var fableScripts = funnel(transpiled, {
  include   : ['**/*.js'],
  destDir: 'out',
});
var main = concat(fableScripts, {
  inputFiles: "**/*.js",
  outputFile: '/js/web.js'
});


var vendorJs = funnel('node_modules', {
  srcDir  : '/',
  include   : ['core-js/client/*',
             'requirejs/*',
             'fable-core/*'
            ],
  destDir: '/js'
});

module.exports = mergeTrees([appHtml, main, vendorJs]);
