SELECT u."Username", r."Name" as "RoleName"
FROM "Users" u
JOIN "Roles" r ON u."RoleId" = r."Id";
