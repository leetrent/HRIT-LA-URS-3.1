select
	u.Id as UserId,
	u.UserName,
    r.Id as RoleId,
    r.Name as RoleName
from 
	URS.AspNetUsers u,
    URS.AspNetUserRoles ur,
    URS.AspNetRoles r
where
	ur.UserId = u.Id
and
	r.Id = ur.RoleId
order by
	u.Username,
    r.Name;
	