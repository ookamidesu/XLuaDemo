require("Game.UI.Login.LoginModel");





---@class LoginController : BaseController
local LoginController = BaseClass(BaseController)

function LoginController:Init()
    ---@type BaseView
    --- require 加载只要不卸载,只自行一次.及时内部定义local变量返回的也是相同值
    self.view = require("Game.UI.Login.LoginView").New();

    UIManager:GetInstance():CreatePanel(self.view)
end

return LoginController