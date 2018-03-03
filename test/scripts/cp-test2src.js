const unpm = require('unity-npm-utils');
const path = require('path');

const unitySrcRoot = path.join(__dirname, '..');
const tgtPkgRoot = path.join(unitySrcRoot, '..');

unpm.copyFromUnity2PkgRoot(unitySrcRoot, tgtPkgRoot)
    .then(info => {
        const pkgName = (info && info.package) ?
            info.package.name || '[package name unset]' :
            '[package info missing from result]';

        console.log(`cp-test2src succeeded for package ${pkgName}`);
    })
    .catch(err => {
        console.log('cp-test2src failed with error: %j', err);
        process.exit(1);
    })
