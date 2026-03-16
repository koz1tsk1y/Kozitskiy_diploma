# Ключевые сущности, связи и API (эскиз для ИС «Дорожная разметка»)

В этом разделе я спроектировал структуру базы данных и REST API для связи фронтенда (Blazor Server / мобильный клиент) с бэкендом на ASP.NET Core.

### Сущности (основные)

* **User** (Пользователи / Сотрудники)
  * id: UUID
  * login: string (unique)
  * password_hash: string
  * full_name: string (ФИО или название бригады)
  * role: enum [admin, dispatcher, foreman]
* **WorkType** (Справочник видов работ)
  * id: UUID
  * name: string (например, "Разметка 1.14.1 Пешеходный переход")
  * unit: string (единица измерения, например "м2" или "км")
* **Job** (Заявка на разметку)
  * id: UUID
  * customer: string (Заказчик)
  * work_type_id: reference -\> WorkType.id
  * geo_data: string/JSON (координаты маркера или полилинии из Leaflet)
  * plan_date: datetime (плановая дата начала)
  * status: enum [new, scheduled, in_progress, completed, closed]
  * created_by: reference -\> User.id (Кто создал - Диспетчер)
  * foreman_id: reference -\> User.id (Исполнитель - Бригада, opt)
  * materials_fact: string (Фактический расход, заполняется мастером)
* **Attachment** (Фотоотчеты с объекта)
  * id: UUID
  * job_id: reference -\> Job.id
  * file_path: string (путь к файлу в хранилище)
  * uploaded_at: datetime
* **Act** (Акт выполненных работ)
  * id: UUID
  * job_id: reference -\> Job.id (unique)
  * generated_at: datetime
  * document_url: string (ссылка на сгенерированный PDF)

### Связи (ER-эскиз)

* **User** (Dispatcher) 1..\* **Job** (создает заявки)
* **User** (Foreman) 1..\* **Job** (выполняет заявки)
* **WorkType** 1..\* **Job** (заявка ссылается на тип работ)
* **Job** 1..\* **Attachment** (к заявке прикрепляются фотоотчеты)
* **Job** 1..1 **Act** (на одну закрытую заявку генерируется один акт)

**Обязательные ограничения (Constraints):**

* unique(User.login)
* Job.work_type_id → WorkType.id (FK, not null)
* Attachment.job_id → Job.id (FK, not null, ON DELETE CASCADE)
* Act.job_id → Job.id (FK, not null, unique)

---

### API - верхнеуровневые ресурсы и операции

**Общие принципы, которые я заложил в архитектуру:**

* Ответы обернуты в стандартную структуру: `{ "status": "ok" | "error", "data": ..., "error_message": "..." }`
* Аутентификация: `Authorization: Bearer <jwt>` (или встроенная кука ASP.NET Core Identity для Blazor).
* Ролевая модель: контроллеры защищены атрибутами `[Authorize(Roles = "admin, dispatcher")]`.

**1. Auth (Аутентификация)**

* POST `/api/auth/login` - `{login, password}` → `200 {token, user_info}`

**2. Users (Управление персоналом)**

* GET `/api/users` - Admin, Dispatcher (список для назначения бригад)
* POST `/api/users` - Admin (добавление нового сотрудника)
* PUT `/api/users/{id}` - Admin

**3. WorkTypes (Справочники)**

* GET `/api/work-types` - Все авторизованные (для выпадающих списков)
* POST `/api/work-types` - Admin (добавление нового типа разметки)

**4. Jobs (Управление заявками и планирование)**

* GET `/api/jobs?status=&foreman_id=&from_date=&to_date=` - получение списка (с фильтрами для календаря и таблицы)
* POST `/api/jobs` - Dispatcher (создание заявки)
  * *Payload (пример):* `{"customer": "ДЭУ-1", "work_type_id": "uuid", "geo_data": "[52.097, 23.734]"}`
* GET `/api/jobs/{id}` - детали заявки (включая прикрепленные фото)
* PUT `/api/jobs/{id}` - Dispatcher (редактирование, drag&drop в календаре меняет дату и `foreman_id`)
* PATCH `/api/jobs/{id}/status` - Foreman, Dispatcher (смена статуса, например, на `completed`)

**5. Attachments (Фотоотчеты)**

* POST `/api/jobs/{id}/attachments` - Foreman (загрузка фото с мобильного телефона, multipart/form-data)

**6. Acts (Документооборот)**

* POST `/api/jobs/{id}/act/generate` - Dispatcher (запуск генерации PDF-акта) → `201 {document_url}`
* GET `/api/acts` - список всех сгенерированных актов для выгрузки.

---

### Критерии приёмки (AC) для основного бизнес-процесса

* **AC1:** При создании заявки (POST `/api/jobs`) диспетчер обязан передать геометку, иначе API возвращает `400 Bad Request`.
* **AC2:** Мастер (роль `foreman`) при запросе GET `/api/jobs` получает только те объекты, где `foreman_id` равен его собственному ID (ограничение scope на уровне EF Core / базы данных).
* **AC3:** Перевести заявку в статус `completed` (PATCH `/api/jobs/{id}/status`) можно только если в базе есть хотя бы одна запись в таблице `Attachment` для этого `job_id` (защита от закрытия объекта без фотоотчета).