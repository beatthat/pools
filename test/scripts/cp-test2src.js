const unpm = require('unity-npm-utils');
const path = require('path');

const unitySrcRoot = path.join(__dirname, '..');
const tgtPkgRoot = path.join(unitySrcRoot, '..');

unpm.syncPlugin2Src(unitySrcRoot, tgtPkgRoot);
