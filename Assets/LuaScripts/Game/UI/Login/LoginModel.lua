
---@class LoginModel : BaseModel
LoginModel = BaseClass(BaseModel)


function LoginModel:SetGameAnnounce(value)
    self.gameAnnounce = value
end

function LoginModel:GetGameAnnounce()
    return self.gameAnnounce
end
