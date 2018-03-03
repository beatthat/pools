const unpm = require('unity-npm-utils');
const path = require('path');

const pkgRoot = path.join(__dirname, '..');

unpm.pkg2UnityInstall(pkgRoot, (err, info) => {
    if(err) {
        console.error('install to unity failed with error: ', err);
        return;
    }

    console.log(`installed to ${info.unity_install_path}`);
});
