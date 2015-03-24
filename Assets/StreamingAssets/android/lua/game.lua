require "define"
require "functions"
require "TestTransmitter"

Log = com.gt.GTLib.GameManager.Log
NetManager = com.gt.GTLib.NetManager
--管理器--
local this;
GameManager = {};

--初始化完成，发送链接服务器信息--
function GameManager.OnInit()
	this = GameManager;
	Log:Info("测试!");
	--注册消息发包器--
	NetManager:AddTransmitter("test",LuaMessageTransmitter.New(1,"TestTransmitter"));
	
	--连接服务器--
	TestTransmitter:Connect("127.0.0.1",9934);
	
	--添加事件--
	NetManager:AddLuaEventListener("conn1",this.OnConnect);
	--测试删除事件--
	--NetManager:RemoveLuaEventListener("conn1",this.OnConnect);
	NetManager:AddLuaEventListener("lost1",this.OnLost);
	NetManager:AddLuaEventListener("login1",this.OnLogin);
	NetManager:AddLuaEventListener("le1",this.OnLoginFail);
end

--连接事件处理--
function GameManager.OnConnect(evt)
	Log:Info("连接成功!");
	Log:Info("test2 : " .. evt["test2"]);
	Log:Info("test3 : " .. evt["test3"]);
	TestTransmitter:Login("ggg","ggg");
end

--连接失败事件处理--
function GameManager.OnLost()
	Log:Info("连接失败!");
end

--登陆成功事件处理--
function GameManager.OnLogin()
	Log:Info("登陆成功!");
	TestTransmitter:CustomTest();
end

--登陆失败事件处理--
function GameManager.OnLoginFail()
	Log:Info("登陆失败!");
end

--销毁--
function GameManager.OnDestroy()
	--warn('OnDestroy--->>>');
end
