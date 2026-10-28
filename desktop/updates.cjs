'use strict';
// Download quietly; electron-updater applies the verified installer on normal exit.
function createUpdateService({app,updater,getWindow,dialog,timers=globalThis}) {
  let pending=null,initial=null,periodic=null,ready=false,started=false;
  updater.setFeedURL({provider:'github',owner:'Mousquoto',repo:'Yuro-Windows',private:false,releaseType:'release'});
  updater.autoDownload=true;
  updater.autoInstallOnAppQuit=true;
  updater.autoRunAppAfterInstall=false;
  updater.allowDowngrade=false;
  updater.disableDifferentialDownload=true;
  function progress(value){const win=getWindow();if(win&&!win.isDestroyed())win.setProgressBar?.(value);}
  function message(text){const win=getWindow();if(win&&!win.isDestroyed())dialog.showMessageBox(win,{type:'info',title:'Mises à jour YURO',message:text});}
  updater.on('download-progress',event=>progress(Math.max(0,Math.min(1,event.percent/100))));
  updater.on('update-downloaded',()=>{ready=true;progress(-1);});
  updater.on('error',()=>progress(-1));
  function check(manual=false){
    if(!app.isPackaged){if(manual)message('Les mises à jour sont disponibles dans l’application installée.');return Promise.resolve();}
    if(pending)return pending;
    if(ready){if(manual)message('La mise à jour est prête. Elle sera installée automatiquement quand tu quitteras YURO.');return Promise.resolve();}
    pending=Promise.resolve().then(()=>updater.checkForUpdates()).then(result=>{
      if(manual)message(result?.updateInfo?.version&&result.updateInfo.version!==app.getVersion()?'La nouvelle version se télécharge en arrière-plan. Tu peux continuer à jouer.':'Ton application est à jour.');
    }).catch(()=>{if(manual)message('Impossible de vérifier les mises à jour pour le moment. YURO réessaiera automatiquement.');}).finally(()=>{pending=null;});
    return pending;
  }
  function start(){if(started||!app.isPackaged)return;started=true;initial=timers.setTimeout(()=>check(),12000);periodic=timers.setInterval(()=>check(),20*60*1000);initial?.unref?.();periodic?.unref?.();}
  function stop(){timers.clearTimeout(initial);timers.clearInterval(periodic);initial=periodic=null;started=false;}
  return {start,stop,check};
}
module.exports={createUpdateService};
