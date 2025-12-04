-- Bu script mevcut Doctor rolündeki kullanıcılar için Doctor kaydı oluşturur
-- Eğer zaten Doctor kaydı varsa, tekrar oluşturmaz

INSERT INTO Doctors (Id, UserId, Specialization, LicenseNumber, YearsOfExperience, HourlyRate, IsApproved, CreatedAt, UpdatedAt)
SELECT 
    NEWID() as Id,
    u.Id as UserId,
    'General' as Specialization,
    '' as LicenseNumber,
    0 as YearsOfExperience,
    0 as HourlyRate,
    0 as IsApproved,
    GETUTCDATE() as CreatedAt,
    GETUTCDATE() as UpdatedAt
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.Name = 'Doctor'
AND NOT EXISTS (
    SELECT 1 FROM Doctors d WHERE d.UserId = u.Id
);

-- Bu script mevcut Patient rolündeki kullanıcılar için Patient kaydı oluşturur
INSERT INTO Patients (Id, UserId, CreatedAt, UpdatedAt)
SELECT 
    NEWID() as Id,
    u.Id as UserId,
    GETUTCDATE() as CreatedAt,
    GETUTCDATE() as UpdatedAt
FROM AspNetUsers u
INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
WHERE r.Name = 'Patient'
AND NOT EXISTS (
    SELECT 1 FROM Patients p WHERE p.UserId = u.Id
);
