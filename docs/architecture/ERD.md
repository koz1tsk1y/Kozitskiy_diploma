# ERD (диаграмма сущностей) - ИС «Дорожная разметка»

В этом разделе представлена структура базы данных проектируемой системы. Файл содержит: 1) mermaid-диаграмму ERD; 2) ASCII-эскиз для быстрого понимания связей; 3) SQL DDL-скетч для инициализации таблиц (на примере PostgreSQL, который отлично работает в связке с EF Core).

## Mermaid ERD

``` Фрагмент кода
erDiagram
    USER ||--o{ JOB : "creates (dispatcher) / executes (foreman)"
    WORK_TYPE ||--o{ JOB : "classifies"
    JOB ||--o{ ATTACHMENT : "has_photos"
    JOB ||--o| ACT : "generates"

    USER {
      uuid id PK
      string login
      string password_hash
      string full_name
      string role
    }
    WORK_TYPE {
      uuid id PK
      string name
      string unit
    }
    JOB {
      uuid id PK
      string customer
      uuid work_type_id FK
      string geo_data
      datetime plan_date
      string status
      uuid created_by FK
      uuid foreman_id FK
      string materials_fact
    }
    ATTACHMENT {
      uuid id PK
      uuid job_id FK
      string file_path
      datetime uploaded_at
    }
    ACT {
      uuid id PK
      uuid job_id FK
      datetime generated_at
      string document_url
    }
```

## ASCII-эскиз

``` Plaintext
User (Dispatcher/Foreman) 1---* Job *---1 WorkType
                                 |
                                 +---* Attachment (Photo reports)
                                 |
                                 +---1 Act (Generated PDF)
```

## SQL DDL (инициализация структуры БД)

Ниже представлен SQL-код для создания таблиц, который будет сгенерирован с помощью миграций Entity Framework Core. Добавлены базовые ограничения целостности (Foreign Keys) и каскадное удаление там, где это необходимо.

``` SQL
CREATE TABLE users (
  id UUID PRIMARY KEY,
  login TEXT UNIQUE NOT NULL,
  password_hash TEXT NOT NULL,
  full_name TEXT NOT NULL,
  role TEXT NOT NULL CHECK (role IN ('admin','dispatcher','foreman'))
);

CREATE TABLE work_types (
  id UUID PRIMARY KEY,
  name TEXT NOT NULL,
  unit TEXT NOT NULL
);

CREATE TABLE jobs (
  id UUID PRIMARY KEY,
  customer TEXT NOT NULL,
  work_type_id UUID NOT NULL REFERENCES work_types(id) ON DELETE RESTRICT,
  geo_data TEXT NOT NULL,
  plan_date TIMESTAMP WITH TIME ZONE NOT NULL,
  status TEXT NOT NULL DEFAULT 'new',
  created_by UUID NOT NULL REFERENCES users(id),
  foreman_id UUID REFERENCES users(id),
  materials_fact TEXT
);

CREATE TABLE attachments (
  id UUID PRIMARY KEY,
  job_id UUID NOT NULL REFERENCES jobs(id) ON DELETE CASCADE,
  file_path TEXT NOT NULL,
  uploaded_at TIMESTAMP WITH TIME ZONE DEFAULT now()
);

CREATE TABLE acts (
  id UUID PRIMARY KEY,
  job_id UUID NOT NULL REFERENCES jobs(id) ON DELETE CASCADE,
  generated_at TIMESTAMP WITH TIME ZONE DEFAULT now(),
  document_url TEXT NOT NULL,
  UNIQUE(job_id) -- Гарантирует связь 1 к 1 (на одну заявку только один акт)
);
```