const {app,BrowserWindow,Menu,shell,dialog}=require('electron');
const path=require('node:path');
const {createUpdateService}=require('./updates.cjs');
const {autoUpdater}=require('electron-updater');
const SITE='https://yurocollection.fr/';
const TRUSTED=new Set(['https://yuro-rho.vercel.app','https://yurocollection.fr','https://discord.com','https://www.discord.com']);
let win;
const updates=createUpdateService({app,updater:autoUpdater,getWindow:()=>win,dialog});
function safeExternal(raw){try{const u=new URL(raw);if(['https:','http:'].includes(u.protocol))shell.openExternal(u.href);}catch{}}
function permitted(raw){try{return TRUSTED.has(new URL(raw).origin);}catch{return false;}}
function checkUpdates(manual=false){return updates.check(manual);}
function openSite(){win.loadURL(SITE+'#boosters').catch(()=>{});}
function createWindow(){win=new BrowserWindow({width:1440,height:960,frame:false,fullscreen:true,autoHideMenuBar:true,minWidth:900,minHeight:650,title:'YURO',backgroundColor:'#000000',icon:path.join(__dirname,'assets/yuro.ico'),show:false,webPreferences:{nodeIntegration:false,contextIsolation:true,sandbox:true,webSecurity:true,allowRunningInsecureContent:false}});win.once('ready-to-show',()=>win.show());
 win.webContents.setUserAgent(win.webContents.getUserAgent()+' YURODesktop/'+app.getVersion());
 win.webContents.setWindowOpenHandler(({url})=>{if(permitted(url)){win.loadURL(url);return {action:'deny'};}safeExternal(url);return {action:'deny'};});
 win.webContents.on('will-navigate',(event,url)=>{if(!permitted(url)){event.preventDefault();safeExternal(url);}});
 win.webContents.session.setPermissionRequestHandler((_wc,_permission,callback)=>callback(false));
 win.webContents.on('will-attach-webview',event=>event.preventDefault());
 win.webContents.on('did-fail-load',(_event,code,_description,url,isMainFrame)=>{if(isMainFrame&&code!==-3&&url.startsWith(SITE))win.loadFile(path.join(__dirname,'loading.html'));});
 win.webContents.on('before-input-event',(e,input)=>{if(input.type!=='keyDown')return;const key=input.key.toLowerCase();if(key==='f11'){e.preventDefault();win.setFullScreen(!win.isFullScreen());}else if(key==='f4'&&input.alt){e.preventDefault();win.close();}else if(key==='f5'||key==='r'&&input.control){e.preventDefault();win.reload();}else if(key==='u'&&input.control&&input.shift){e.preventDefault();checkUpdates(true);}});
 Menu.setApplicationMenu(null);openSite();updates.start();}
if(!app.requestSingleInstanceLock())app.quit();else{app.on('second-instance',()=>{if(win){if(win.isMinimized())win.restore();win.focus();}});app.whenReady().then(createWindow);app.on('window-all-closed',()=>app.quit());}
app.on('before-quit',()=>updates.stop());
