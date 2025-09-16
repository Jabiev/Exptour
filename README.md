# 🌍 ExpTour App - Backend Configuration

This repository contains the backend configuration for the **ExpTour App**, a multilingual tour guide platform with features such as OTP verification, multilingual messaging, and integrations with RabbitMQ, Redis, PostgreSQL, Google Authentication, and Elasticsearch.

---    Brief Summary

## 🚀 Features

- **PostgreSQL** integration for data persistence.
- **Redis** for caching and performance optimization.
- **RabbitMQ** for asynchronous messaging and microservices.
- **Serilog** for advanced logging to Console and Seq.
- **JWT Authentication** with refresh token support.
- **Multilingual error messaging** (English & Arabic).
- **ElasticSearch** integration for indexing and fast querying.
- **OTP system** for secure user verification.
- **Google OAuth2** login integration.
- **Mail system** via Gmail SMTP.
- **Violation rules** for guide scheduling policies.    And more...

---

## 📦 Tech Stack

| Component       | Technology           |
|-----------------|--------------------|
| **Backend**     | ASP.NET Core 9 Web API |
| **Database**    | PostgreSQL |
| **Caching**     | Redis |
| **Messaging**   | RabbitMQ |
| **Search**      | ElasticSearch |
| **Logging**     | Serilog + Seq |
| **Auth**        | JWT + Google OAuth2 |
| **Email**       | Gmail SMTP |

---

🗂 Multilingual App Messages
All error and system messages are available in English and Arabic. These include:

- Login failures

- Token errors

- OTP validation

- Guide schedule violations

- Language validation

- Mail notifications

And more...

---

## ⚙️ Example `appsettings.Development.json`

> All secrets have been removed and replaced with `XXX` for security.  
> Use your own credentials when deploying.

```jsonc
{
  "Logging": {
    "LogLevel": {
      "Default": "XXX",
      "Microsoft.AspNetCore": "XXX"
    }
  },
  "ConnectionStrings": {
    "PostgreSQL": "XXX"
  },
  "RabbitMQ": {
    "HostName": "XXX",
    "UserName": "XXX",
    "Password": "XXX",
    "VirtualHost": "XXX",
    "Port": "XXX",
    "UseSSL": "XXX"
  },
  "OTPGenerate": {
    "OTPRangeFrom": "XXX",
    "OTPRangeTo": "XXX",
    "ExpireMinute": "XXX",
    "ResendMinute": "XXX",
    "AttemptCount": "XXX",
    "Email": {
      "EmailSubjectEn": "XXX",
      "EmailSubjectAr": "XXX",
      "EmailBodyEn": "XXX",
      "EmailBodyAr": "XXX"
    }
  },
  "Redis": {
    "ConnectionString": "XXX"
  },
  "Serilog": {
    "Using": ["XXX", "XXX", "XXX"],
    "MinimumLevel": {
      "Default": "XXX",
      "Override": {
        "System": "XXX",
        "Microsoft": "XXX",
        "Microsoft.AspNetCore": "XXX",
        "Microsoft.EntityFrameworkCore": "XXX"
      }
    },
    "WriteTo": [
      { "Name": "XXX" },
      {
        "Name": "XXX",
        "Args": { "serverUrl": "XXX" }
      }
    ],
    "Properties": {
      "Application": "XXX"
    }
  },
  "ApplicationUrl": "XXX",
  "JWTSettings": {
    "SecurityKey": "XXX",
    "Issuer": "XXX",
    "Audience": "XXX",
    "AccessTokenExpirationMinutes": "XXX",
    "RefreshTokenExpirationMinutes": "XXX"
  },
  "GoogleSettings": {
    "ClientId": "XXX",
    "ClientSecret": "XXX"
  },
  "DetectLanguageAPI": {
    "ApiKey": "XXX",
    "ApiUrl": "XXX"
  },
  "ElasticSearch": {
    "Uri": "XXX",
    "Username": "XXX",
    "Password": "XXX",
    "DefaultIndex": "XXX"
  },
  "CloudinarySettings": {
    "CloudName": "XXX",
    "ApiKey": "XXX",
    "ApiSecret": "XXX"
  },
  "Mail": {
    "Username": "XXX",
    "DisplayName": "XXX",
    "Password": "XXX",
    "Host": "XXX"
  },
  "GuideViolations": {
    "MaxDaysBetweenSchedules": "XXX",
    "MaxAllowedOffDays": "XXX",
    "BasicViolation": "XXX"
  }
}

📬 Contact
For any inquiries, reach out to:
Email: exptourbyjabiev@gmail.com
Developer: J A B I E V
```

## 🛠 Example `appsettings.json`

Below is a simplified `appsettings.json` example you can use as a reference.
```jsonc
"AppMessages": {
  "InvalidLogin": {
    "en": "Invalid login, please try again",
    "ar": "تسجيل الدخول غير صالح",
    "state": "invalid-credentials"
  },
  "InvalidRequest": {
    "en": "Invalid datas which come from input",
    "ar": "البيانات غير صالحة التي تأتي من الإدخال",
    "state": "invalid-request"
  },
  "InvalidUserToken": {
    "en": "The user or the token provided is invalid",
    "ar": "المستخدم أو الرمز المميز المقدم غير صالح",
    "state": "invalid-user-token"
  },
  "InvalidResetToken": {
    "en": "The reset token provided is invalid",
    "ar": "الرمز المميز المقدم غير صالح",
    "state": "invalid-reset-token"
  },
  "InvalidToken": {
    "en": "The token provided is invalid",
    "ar": "الرمز المميز المقدم غير صالح",
    "state": "invalid-token"
  },
  "InvalidDate": {
    "en": "The date provided is invalid",
    "ar": "التاريخ المقدم غير صالح",
    "state": "invalid-date"
  },
  "InvalidOTP": {
    "en": "The OTP provided is invalid",
    "ar": "الرمز المميز المقدم غير صالح",
    "state": "invalid-otp"
  },
  "AttemptExceeded": {
    "en": "The number of attempts exceeded",
    "ar": "تم تجاوز عدد المحاولات",
    "state": "attempt-exceeded"
  },
  "TooManyRequests": {
    "en": "Too many requests",
    "ar": "تم تجاوز عدد المحاولات",
    "state": "too-many-requests"
  },
  "UnauthorizedUser": {
    "en": "The User is unauthorized",
    "ar": "المستخدم غير مصرح به",
    "state": "unauthorized-user"
  },
  "UserCanNotBeFound": {
    "en": "The User can not be found",
    "ar": "لا يمكن العثور على المستخدم",
    "state": "user-can-not-be-found"
  },
  "CanNotBeEmpty": {
    "en": "Can not be Empty",
    "ar": "لا يمكن أن يكون فارغاً",
    "state": "cannot-be-empty"
  },
  "EmailInvalidFormat": {
    "en": "Your email address is not valid",
    "ar": "عنوان بريدك الإلكتروني غير صالح",
    "state": "email-invalid-format"
  },
  "Success": {
    "en": "Request executed successfully",
    "ar": "تم تنفيذ الطلب بنجاح",
    "state": "success"
  },
  "MailSentSuccessfully": {
    "en": "The mail sent successfully",
    "ar": "تم إرسال البريد بنجاح",
    "state": "mail-sent-successfully"
  },
  "MailCanNotBeSent": {
    "en": "The mail can not be sent",
    "ar": "لا يمكن ارسال البريد",
    "state": "mail-can-not-be-sent"
  },
  "NotFound": {
    "en": "We couldn't find the requested information",
    "ar": "لا يمكن العثور على المعلومات المطلوبة",
    "state": "not-found"
  },
  "ActionNotFound": {
    "en": "Action doesn't found",
    "ar": "العملية غير موجودة",
    "state": "action-not-found"
  },
  "ScheduleNotFound": {
    "en": "Schedule doesn't found",
    "ar": "الجدول غير موجود",
    "state": "schedule-not-found"
  },
  "ExpiredError": {
    "en": "Expired!",
    "ar": "منتهي الصلاحية!",
    "state": "expired-error"
  },
  "RefreshTokenExpired": {
    "en": "The expireTime of the refreshToken has expired",
    "ar": "لقد انتهت مدة صلاحية رمز التحديث",
    "state": "refresh-token-expired"
  },
  "WrongCode": {
    "en": "Wrong code.You have {attemptCount} attempts remaining",
    "ar": "رمز خاطئ. لديك {attemptCount} محاولات متبقية",
    "state": "wrong-code"
  },
  "WrongPassword": {
    "en": "The Password which is entered is wrong",
    "ar": "كلمة المرور التي تم إدخالها خاطئة",
    "state": "wrong-password"
  },
  "UserDoesNotExist": {
    "en": "The specified user does not exist. Please check the user ID,Email or Phone and try again",
    "ar": "المستخدم المحدد غير موجود. يرجى التحقق من معرف المستخدم أو البريد الإلكتروني أو الهاتف والمحاولة مرة أخرى",
    "state": "user-does-not-exist"
  },
  "OneOrMoreLanguagesCanNotBeFound": {
    "en": "One or more languages were not found",
    "ar": "واحد او اكثر من الكيانات غير موجودة",
    "state": "one-or-more-can-not-found"
  },
  "LanguagesCanNotBeAssigned": {
    "en": "Languages can not be assigned",
    "ar": "لا يمكن تعيين اللغات",
    "state": "languages-can-not-be-assigned"
  },
  "ThereAreNoNewLanguages": {
    "en": "There are no new languages for assigning",
    "ar": "لا يوجد لغات جديدة",
    "state": "there-are-no-new-languages"
  },
  "ExceedingOffDays": {
    "en": "Exceeding off days",
    "ar": "تجاوز اليوم",
    "state": "exceeding-off-days"
  },
  "GuideOnTourOrWillBe": {
    "en": "The guide is on tour or will be on tour",
    "ar": "المرشد في الرحلة او سوف يكون في الرحلة",
    "state": "guide-on-tour-or-will-be"
  },
  "MustHaveAtLeastTwoLanguages": {
    "en": "Any guide must have at least two languages",
    "ar": "يجب ان يكون احد المرشدين على الاقل ثلاثة لغات",
    "state": "any-guide-must-have-at-least-two-languages"
  },
  "UserDoesNotActive": {
    "en": "The user doesn't active",
    "ar": "المستخدم غير نشط",
    "state": "user-does-not-active"
  },
  "OTPAlreadySent": {
    "en": "OTP already sent",
    "ar": "OTP مرسل بالفعل",
    "state": "otp-already-sent"
  },
  "PendingVerification": {
    "en": "Pending verification",
    "ar": "قيد التحقق",
    "state": "pending-verification"
  },
  "AlreadyExists": {
    "en": "The entity already exists",
    "ar": "الكيان موجود بالفعل",
    "state": "already-exists"
  },
  "AlreadyAssignedInThisPeriod": {
    "en": "The schedule already assigned in this period or something that might be overlap",
    "ar": "الجدول موجود بالفعل في هذا الفترة",
    "state": "already-assigned-in-this-period"
  },
  "EntityHasAssociatedEntities": {
    "en": "The entity has associated entities",
    "ar": "الكيان يحتوي على كيانات مرتبطة",
    "state": "entity-has-associated-entities"
  },
  "NoEntityAssociation": {
    "en": "No Entity association with the id you sent",
    "ar": "لا يوجد ارتباط للكيان بمعرف المستخدم",
    "state": "no-entity-association"
  },
  "NoRolesAssigned": {
    "en": "No Roles Assigned",
    "ar": "لم يتم تعيين أي أدوار",
    "state": "no-roles-assigned"
  },
  "NoRolesAssignedToUser": {
    "en": "No Roles Assigned to User",
    "ar": "لم يتم تعيين أي أدوار للمستخدم",
    "state": "no-roles-assigned"
  },
  "NoNewRolesToAssign": {
    "en": "There are No New Roles to Assign",
    "ar": "لا توجد أدوار جديدة لتعيينها",
    "state": "no-roles-assigned"
  },
  "RoleDoesNotExist": {
    "en": "Even one Role you entered does not exist. Please check properly and try again",
    "ar": "حتى الدور الذي أدخلته غير موجود. يرجى التحقق بشكل صحيح ثم المحاولة مرة أخرى",
    "state": "role-does-not-exist"
  },
  "RolesCanNotBeFound": {
    "en": "Any Role can't find",
    "ar": "لا يمكن العثور على أي دور",
    "state": "role-does-not-exist"
  },
  "RoleAlreadyExists": {
    "en": "The role already exists",
    "ar": "الكيان موجود بالفعل",
    "state": "role-already-exists"
  },
  "RoleInUse": {
    "en": "The role is currently in use",
    "ar": "الدور قيد الاستخدام حاليًا",
    "state": "role-in-use"
  },
  "RoleAccessDenied": {
    "en": "The role denied",
    "ar": "الكيان موجود بالفعل",
    "state": "role-access-denied"
  },
  "SomethingWentWrong": {
    "en": "Something went wrong. Please try again.",
    "ar": "حدث خطأ ما. يرجى المحاولة مرة أخرى",
    "state": "error-something-went-wrong"
  },
  "Failure": {
    "en": "The Process is failed",
    "ar": "فشل",
    "state": "failure"
  }
}
