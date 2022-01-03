require("Game.UI.Login.LoginModel");


---@class LoginController : BaseController
local LoginController = BaseClass(BaseController)

function LoginController:Init()
    ---@type LoginView
    --- require 加载只要不卸载,只自行一次.及时内部定义local变量返回的也是相同值
    self.view = require("Game.UI.Login.LoginView").New();

    UIManager:GetInstance():CreatePanel(self.view)
    
    self.view:Init(self)
    
end

---Login
---@param user string
---@param psw string
function LoginController:StartLogin(user,psw)
    
    --发送网络请求
    NetMgr:SendMessage("123", function()
        self:OnReceiveMsg()
    end );
end

function LoginController:OnReceiveMsg( )
    print("登录成功")
    UIManager:GetInstance():Hide(self.view)
    
end

return LoginController