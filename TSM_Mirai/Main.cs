using Maila.Cocoa.Framework;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TSM_Mirai;
[BotModule]
public class Main : BotModuleBase
{
    public static string Version="v1.0.0.0";
    public static string LastUpdateTime = "2021.7.24 15:27";
    protected override bool OnMessage(MessageSource src, QMessage msg)
    {
        //src.Send("hi");
        var Msg = msg.PlainText;
        //src.Send(Msg);
        if(src.IsFriend)
        {
            //src.Send(Utils.GetNick(src.User.Id));
        }
        if (src.IsGroup&&Utils.IsGroup(src.Group.Id))
        {
            void Send(string text)
            {
                src.Send(text);
            }
            if (Msg == "在线")
            {
                try
                {
                    var config = Config.GetConfig();
                    Task showstatus = new Task(() =>
                    {
                        config.Servers.ToList().ForEach(s =>
                        {
                            try
                            {
                                Send(Utils.Status(s, false));
                                //Api.ApiSendMsg(RobotQQ, MsgType, MsgFrom, MsgFrom, Tools.Status(s, false), 3996);
                            }
                            catch (Exception ex)
                            {
                                //Api.ApiSendMsg(RobotQQ, MsgType, MsgFrom, MsgFrom, ex.ToString(), 3996);
                            }
                        });
                    });
                    showstatus.Start();
                }
                catch (Exception ex)
                {
                    //Api.ApiSendMsg(RobotQQ, MsgType, MsgFrom, MsgFrom, ex.ToString(), 3996);
                }
            }
            else if (Msg == "关于")
            {
                string str = "欢迎使用TSM_Mirai";
                str += $"\r版本：{Version}";
                str += $"\r本软件由Fishing Studio(一个人的工作室)——Leader独立开发";
                str += $"\r本软件为开源免费软件，开源地址：https://github.com/Leader-txt/TSMMirai";
                str += $"\r本bot软件随功能较少，但麻雀虽小，五脏俱全，后续如果没咕的话还会继续更新，敬请期待！";
                str += $"\r最后编辑时间{LastUpdateTime}";
                src.Send(str);
            }
            else if (Msg == "帮助")
            {
                string Info = "欢迎使用TSM_Mirai " + Version + " 以下是使用说明\r";
                Info += "输入'在线'获取所有服务器当前状态\r";
                Info += "'输入'关于'获取本软件信息\r";
                Info += "输入'say 服务器名(指定为所有服务器则为all) 要说的话'向目标服务器喊话\r";
                Info += "输入'pack 服务器名 玩家名'查询指定玩家背包数据";
                if(Utils.IsAdmin (src.User.Id))
                {
                    Info += "\r输入'cmd 服务器名(指定为所有服务器则为all) 命令(如/help,/help 2)'对指定服务器执行命令";
                }
                Send(Info);
            }
            else if(Msg.StartsWith("say "))
            {
                new Task(() =>
                {
                    try
                    {
                        var strs = Msg.Split(' ');
                        var words =Utils.GetNick(src.User.Id)+"说:"+ strs[2] + " ";
                        for (int i = 3; i < strs.Length; i++)
                        {
                            words += strs[i] + " ";
                        }
                        var server = strs[1];
                        if (server == "all")
                        {
                            foreach (var s in Config.GetConfig().Servers)
                            {
                                Utils.RawCmd(s, "/me " + words);
                                Send($"成功向{s.Name }发送:{words}" );
                                //Api.ApiSendMsg(RobotQQ, MsgType, MsgFrom, MsgFrom, s.Name + "执行命令：" + cmd + "返回数据如下\r" + Tools.RawCmd(s, cmd), 3996);
                            }
                        }
                        else
                        {
                            Utils.RawCmd(strs[1], "/me " + words);
                            Send($"成功向{strs[1]}发送:{words}");
                            //Api.ApiSendMsg(RobotQQ, MsgType, MsgFrom, MsgFrom, strs[1] + "执行命令：" + cmd + "返回数据如下\r" + Tools.RawCmd(strs[1], cmd), 3996);
                            //Api.ApiSendMsg(RobotQQ, MsgType, MsgFrom, MsgFrom, Tools.RawCmd(strs[1], cmd), 3996);
                        }
                    }
                    catch { }

                }).Start();
            }
            else if (Msg.StartsWith("cmd "))
            {
                (new Task(() =>
                {
                    try
                    {
                        if (!Utils.IsAdmin(src.User.Id))
                        {
                            Send("您无权执行命令！");
                            //Api.ApiSendMsg(RobotQQ, MsgType, MsgFrom, MsgFrom, "您无权执行命令！", 3996);
                            return;
                        }
                        var strs = Msg.Split(' ');
                        var cmd = strs[2] + " ";
                        for (int i = 3; i < strs.Length; i++)
                        {
                            cmd += strs[i] + " ";
                        }
                        var server = strs[1];
                        if (server == "all")
                        {
                            foreach (var s in Config.GetConfig().Servers)
                            {
                                Send(s.Name + "执行命令：" + cmd + "返回数据如下\r" + Utils.RawCmd(s, cmd));
                                //Api.ApiSendMsg(RobotQQ, MsgType, MsgFrom, MsgFrom, s.Name + "执行命令：" + cmd + "返回数据如下\r" + Tools.RawCmd(s, cmd), 3996);
                            }
                        }
                        else
                        {
                            Send(strs[1] + "执行命令：" + cmd + "返回数据如下\r" + Utils.RawCmd(strs[1], cmd));
                            //Api.ApiSendMsg(RobotQQ, MsgType, MsgFrom, MsgFrom, strs[1] + "执行命令：" + cmd + "返回数据如下\r" + Tools.RawCmd(strs[1], cmd), 3996);
                            //Api.ApiSendMsg(RobotQQ, MsgType, MsgFrom, MsgFrom, Tools.RawCmd(strs[1], cmd), 3996);
                        }
                    }
                    catch { }
                })).Start();
            }
            else if (Msg.StartsWith("pack "))
            {
                new Task(() =>
                {
                    try
                    {
                        var strs = Msg.Split(' ');
                        var server = Utils.GetServer(strs[1]);
                        if (server == null)
                        {
                            Send("没有找到目标服务器:" + strs[1] + "!");
                            return;
                        }
                        var name = strs[2];
                        string info = "";
                        try
                        {
                            info = Utils.RawCmd(server, "/pack " + name);
                        }
                        catch (Exception ex)
                        {
                            Send("命令执行失败");
                            return;
                        }
                        if (info == "[]")
                        {
                            Send("查无此人！");
                            return;
                        }
                        string[] data = info.Split('\"')[1].Split('~');
                        Bitmap inv = new Bitmap("Items\\background.png");
                        var g = Graphics.FromImage(inv);
                        for (int i = 0; i < data.Length; i++)
                        {
                            //Send(data[i]);
                            int id = int.Parse(data[i].Split(',')[0]), stack = int.Parse(data[i].Split(',')[1]);
                            Bitmap block = new Bitmap("Items\\frame.png");
                            var _g = Graphics.FromImage(block);
                            var item = new Bitmap("Items\\" + id + ".png");
                            _g.DrawImage(item, 30 - item.Width / 2, 30 - item.Height / 2);
                            if (id != 0)
                                _g.DrawString(stack.ToString(), new Font("Arial", 10), new SolidBrush(Color.Black), 5, 40);
                            //宽余10，长余28
                            //x=(i+1)%20;y=(i+1)/20
                            int x = i % 20, y = i / 20;
                            g.DrawImage(block, 14 + (28 + 60) * x, 10 + 70 * y);
                        }
                        //Send("绘制完成");
                        Directory.CreateDirectory("Img");
                        var path = "Img/inv.jpg";
                        inv.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                        byte[] buffer;
                        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                        {
                            using (BinaryReader re = new BinaryReader(fs))
                            {
                                buffer = re.ReadBytes((int)fs.Length);
                            }
                        }
                        //var UUID = Api.ApiUpLoadPic(RobotQQ, MsgType, MsgFrom, buffer);
                        //Api.ApiSendMsg(RobotQQ, MsgType, MsgFrom, RobotQQ, $"玩家:{name}在服务器:{server.Name}的背包数据如下" + UUID, 3996);
                        Send($"玩家:{name}在服务器:{server.Name}的背包数据如下");
                        src.SendImage(path);
                    }
                    catch (Exception ex)
                    {
                        //Api.ApiSendMsg(RobotQQ, MsgType, MsgFrom, MsgFrom, ex.ToString(), 3996);
                    }
                }).Start();
            }
        }
        return true;
    }
}