using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
class YuroSetupForm:Form {
 readonly Color Gold=Color.FromArgb(239,209,141),Muted=Color.FromArgb(158,153,165);readonly Action<Action<int,string>> install;readonly Action launch;readonly bool preview;
 Label title,description,status,percent;SetupButton action;Button close;SetupProgress progress;bool working,complete;Image logo;Timer motion;double phase;
 [DllImport("user32.dll")]static extern bool ReleaseCapture();[DllImport("user32.dll")]static extern IntPtr SendMessage(IntPtr h,int msg,int w,int l);
 public YuroSetupForm(Action<Action<int,string>> installer,Action launcher,bool previewOnly){install=installer;launch=launcher;preview=previewOnly;Text="Installer YURO";Icon=Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);ClientSize=new Size(900,560);FormBorderStyle=FormBorderStyle.None;StartPosition=FormStartPosition.CenterScreen;BackColor=Color.FromArgb(12,12,15);Font=new Font("Segoe UI",10);DoubleBuffered=true;AutoScaleMode=AutoScaleMode.None;
  using(Stream stream=Assembly.GetExecutingAssembly().GetManifestResourceStream("Logo.png")){if(stream!=null)using(Image original=Image.FromStream(stream))logo=new Bitmap(original);}
  MouseDown+=(s,e)=>{if(e.Button==MouseButtons.Left){ReleaseCapture();SendMessage(Handle,0xA1,2,0);}};
  close=new Button{Text="×",Bounds=new Rectangle(849,17,32,32),FlatStyle=FlatStyle.Flat,BackColor=BackColor,ForeColor=Muted,Font=new Font("Segoe UI",17),Cursor=Cursors.Hand,TabIndex=2};close.FlatAppearance.BorderSize=0;close.FlatAppearance.MouseOverBackColor=Color.FromArgb(70,27,37);close.Click+=(s,e)=>{if(!working)Close();};Controls.Add(close);
  Label badge=LabelAt("YURO POUR WINDOWS   /   1.0.3",390,65,430,25,9,Gold,true);Controls.Add(badge);
  title=LabelAt("Ton univers.\nSur ton PC.",385,108,460,115,31,Color.FromArgb(247,245,240),true);Controls.Add(title);
  description=LabelAt("Ouvre tes boosters, rassemble tes héros\net retrouve ta collection dans une application dédiée.",390,240,425,60,11,Muted,false);Controls.Add(description);
  Label destination=LabelAt("INSTALLATION PERSONNELLE",407,324,390,22,8,Gold,true);Controls.Add(destination);Label shortcuts=LabelAt("Raccourcis Bureau et menu Démarrer inclus.",407,350,390,26,10,Color.FromArgb(207,202,211),false);Controls.Add(shortcuts);
  progress=new SetupProgress{Bounds=new Rectangle(390,385,430,9),Visible=false};Controls.Add(progress);status=LabelAt("",390,403,370,24,9,Muted,false);Controls.Add(status);percent=LabelAt("",777,403,45,24,9,Gold,true);percent.TextAlign=ContentAlignment.TopRight;Controls.Add(percent);
  action=new SetupButton{Text="Installer YURO  →",Bounds=new Rectangle(390,444,430,54),TabIndex=0};action.Click+=async(s,e)=>{if(complete){if(!preview)launch();Close();return;}working=true;close.Enabled=false;action.Enabled=false;action.Text="Installation en cours…";title.Text="Un instant.\nL’aventure arrive.";description.Text="Nous préparons YURO sur ton ordinateur.\nTu pourras bientôt rejoindre ta collection.";progress.Visible=true;try{if(preview){for(int i=0;i<=100;i+=4){Report(i,"Préparation de ton univers…");await Task.Delay(60);}}else{await Task.Run(()=>install((n,message)=>{if(!IsDisposed)BeginInvoke(new Action(()=>Report(n,message)));}));}complete=true;working=false;close.Enabled=true;action.Enabled=true;action.Text="Lancer YURO  →";title.Text="Tout est prêt.\nÀ toi de jouer.";description.Text="YURO est installé. Retrouve ton raccourci sur le bureau\nou lance directement ton univers maintenant.";Report(100,"Installation terminée");}catch(Exception ex){working=false;close.Enabled=true;action.Enabled=true;action.Text="Réessayer  →";title.Text="Reprenons\nl’installation.";description.Text="L’installation n’a pas pu se terminer.\nVérifie l’espace disponible et réessaie.";status.Text=ex.Message.Length>65?ex.Message.Substring(0,65)+"…":ex.Message;}};Controls.Add(action);
  Label foot=LabelAt("Connexion internet requise  ·  Sans droits administrateur",390,516,450,20,8,Color.FromArgb(103,100,112),false);Controls.Add(foot);
  FormClosing+=(s,e)=>{if(working)e.Cancel=true;};motion=new Timer{Interval=45};motion.Tick+=(s,e)=>{phase+=.025;Invalidate(new Rectangle(0,0,340,560));};Shown+=(s,e)=>motion.Start();FormClosed+=(s,e)=>{motion.Stop();motion.Dispose();if(logo!=null)logo.Dispose();};
 }
 Label LabelAt(string text,int x,int y,int w,int h,float size,Color color,bool bold){return new Label{Text=text,Bounds=new Rectangle(x,y,w,h),Font=new Font("Segoe UI",size,bold?FontStyle.Bold:FontStyle.Regular),ForeColor=color,BackColor=Color.Transparent,AutoSize=false};}
 void Report(int value,string text){progress.Value=value;status.Text=text;percent.Text=value+" %";}
 protected override void OnPaint(PaintEventArgs e){base.OnPaint(e);Graphics g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;
  using(LinearGradientBrush b=new LinearGradientBrush(new Rectangle(0,0,340,560),Color.FromArgb(34,15,25),Color.FromArgb(12,12,16),100f))g.FillRectangle(b,0,0,340,560);
  using(Pen line=new Pen(Color.FromArgb(47,39,44)))g.DrawLine(line,340,0,340,560);
  for(int i=0;i<4;i++){int size=190+i*58;using(Pen ring=new Pen(Color.FromArgb(32-i*5,239,209,141)))g.DrawEllipse(ring,170-size/2,245-size/2,size,size);}
  using(Pen p=new Pen(Color.FromArgb(110,239,209,141),1.4f)){float y=150+(float)Math.Sin(phase)*8;g.DrawLine(p,48,y,53,y);g.DrawLine(p,50.5f,y-2.5f,50.5f,y+2.5f);g.DrawLine(p,282,320,289,320);g.DrawLine(p,285.5f,316.5f,285.5f,323.5f);}
  if(logo!=null)g.DrawImage(logo,new Rectangle(50,125+(int)(Math.Sin(phase)*3),240,240));
  using(Font f=new Font("Segoe UI",8,FontStyle.Bold))using(Brush b=new SolidBrush(Gold))g.DrawString("LE MULTIVERS TE DONNE RENDEZ-VOUS",f,b,34,48);
  using(Font f=new Font("Segoe UI",19,FontStyle.Bold))using(Brush b=new SolidBrush(Color.FromArgb(239,228,207)))g.DrawString("Tes héros.\nTon histoire.",f,b,47,395);
  using(Font f=new Font("Segoe UI",9))using(Brush b=new SolidBrush(Muted))g.DrawString("Collectionne. Combats. Évolue.",f,b,48,480);
  using(Brush b=new SolidBrush(Color.FromArgb(19,18,22)))g.FillRectangle(b,390,311,430,65);using(Pen p=new Pen(Color.FromArgb(44,39,37)))g.DrawRectangle(p,390,311,430,65);using(Pen p=new Pen(Color.FromArgb(52,43,35)))g.DrawRectangle(p,0,0,899,559);
 }
 public void RenderPreview(string file){ShowInTaskbar=false;Opacity=0;StartPosition=FormStartPosition.Manual;Location=new Point(-2000,-2000);Show();Application.DoEvents();CreateControl();foreach(Control c in Controls)c.CreateControl();using(Bitmap b=new Bitmap(Width,Height)){DrawToBitmap(b,new Rectangle(0,0,Width,Height));b.Save(file,System.Drawing.Imaging.ImageFormat.Png);}Hide();}
}
class SetupButton:Button {
 bool hover;public SetupButton(){SetStyle(ControlStyles.UserPaint|ControlStyles.AllPaintingInWmPaint|ControlStyles.OptimizedDoubleBuffer,true);FlatStyle=FlatStyle.Flat;FlatAppearance.BorderSize=0;Cursor=Cursors.Hand;Font=new Font("Segoe UI",12,FontStyle.Bold);MouseEnter+=(s,e)=>{hover=true;Invalidate();};MouseLeave+=(s,e)=>{hover=false;Invalidate();};}
 protected override void OnPaint(PaintEventArgs e){Graphics g=e.Graphics;g.SmoothingMode=SmoothingMode.AntiAlias;Color a=Enabled?(hover?Color.FromArgb(255,232,176):Color.FromArgb(244,217,155)):Color.FromArgb(89,79,58),b=Enabled?Color.FromArgb(210,167,79):Color.FromArgb(74,65,49);using(GraphicsPath p=Round(new Rectangle(0,0,Width-1,Height-1),12))using(LinearGradientBrush fill=new LinearGradientBrush(ClientRectangle,a,b,0f))g.FillPath(fill,p);TextRenderer.DrawText(g,Text,Font,ClientRectangle,Enabled?Color.FromArgb(29,22,11):Color.FromArgb(185,173,147),TextFormatFlags.HorizontalCenter|TextFormatFlags.VerticalCenter);if(Focused)using(Pen pen=new Pen(Color.FromArgb(255,244,217)))g.DrawRectangle(pen,4,4,Width-9,Height-9);}
 public static GraphicsPath Round(Rectangle r,int radius){GraphicsPath p=new GraphicsPath();int d=radius*2;p.AddArc(r.X,r.Y,d,d,180,90);p.AddArc(r.Right-d,r.Y,d,d,270,90);p.AddArc(r.Right-d,r.Bottom-d,d,d,0,90);p.AddArc(r.X,r.Bottom-d,d,d,90,90);p.CloseFigure();return p;}
}
class SetupProgress:Control {int value;public int Value{get{return value;}set{this.value=Math.Max(0,Math.Min(100,value));Invalidate();}}public SetupProgress(){DoubleBuffered=true;}protected override void OnPaint(PaintEventArgs e){e.Graphics.SmoothingMode=SmoothingMode.AntiAlias;using(Brush b=new SolidBrush(Color.FromArgb(40,35,30)))e.Graphics.FillRectangle(b,ClientRectangle);int w=(int)(Width*value/100.0);if(w>0)using(LinearGradientBrush b=new LinearGradientBrush(ClientRectangle,Color.FromArgb(244,218,160),Color.FromArgb(188,136,48),0f))e.Graphics.FillRectangle(b,0,0,w,Height);}}
