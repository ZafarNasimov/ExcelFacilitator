# ExcelFacilitator

Этот проект изначально был реализован как техническое задание, призванное решить реальную проблему клиента: Excel-файлы с более чем миллионом строк стали слишком медленными для работы — особенно при фильтрации и попытках провести хоть какую-то аналитику. Целью было перенести данные в полноценную базу данных и создать быстрый, надёжный веб-интерфейс для загрузки, фильтрации и аналитики.

Я разработал решение на базе ASP.NET Core MVC с использованием PostgreSQL. Для эффективной загрузки больших Excel-файлов реализован механизм bulk insert. Загрузка файла примерно на миллион строк занимала около 40 секунд. После добавления индексов по часто используемым колонкам — таким как ИНН продавца, ИНН покупателя и код товара — время выросло до 70 секунд. Однако это оправданный компромисс: загрузки происходят редко, а чтение и анализ данных — постоянно и должно быть максимально быстрым.

Система поддерживает гибкую, динамическую фильтрацию и сортировку через AJAX без перезагрузки страницы. Фильтры охватывают широкий спектр параметров: от диапазонов дат и чисел до конкретных атрибутов, таких как ИНН, коды товаров и названия исходных файлов. Пользователь может комбинировать сразу несколько фильтров, что позволяет удобно и быстро находить нужную информацию среди миллионов записей.

Архитектурно проект был построен с использованием подхода Code First, что позволило мне гибко описать структуру данных на уровне C# классов и контролировать миграции через код. Это ускорило процесс моделирования и разработки, особенно на ранних этапах, когда бизнес-правила ещё уточнялись. Такой подход также упростил автоматическую генерацию и версионирование схемы базы данных, что сделало проект легче в сопровождении и расширении.

Кроме фильтрации, я разработал модуль аналитики, полностью основанный на бизнес-потребностях клиента, которые он озвучил в процессе обсуждения. Например, им важно было понимать, какие товары наиболее популярны в разные сезоны, кто из клиентов приносит наибольшую выручку, и есть ли компании, незаконно перепродающие товары. Для этого я реализовал аналитику как в глобальном виде, так и в привязке к установленным фильтрам. Всего в несколько кликов можно получить топ-30 самых продаваемых товаров, самых ценных продаж, либо крупнейших покупателей — как по всей базе, так и по конкретным фильтрам.

Таким образом, я перевёл высокоуровневые бизнес-задачи клиента в конкретные технические решения, обеспечив быструю и понятную аналитику прямо в интерфейсе — без макросов, Excel-фильтров и медленных ручных вычислений. Логика построена с упором на скорость, масштабируемость и соответствие реальным задачам клиента.

Несмотря на минималистичный интерфейс, именно проработанная логика и высокая производительность стали ключевыми факторами, благодаря которым клиент решил привлечь меня к основному проекту.

---

This project was initially a technical task aimed at solving a real pain point for the client: Excel files containing over a million rows were becoming too slow to work with, especially when filtering or performing any meaningful analysis. The goal was to move this data into a proper database and build a fast, reliable web interface for uploads, filtering, and analytics.

I developed the solution using ASP.NET Core MVC with PostgreSQL. To handle large Excel file uploads efficiently, I implemented a bulk insert mechanism. Uploading a file with around 1 million rows took approximately 40 seconds. After indexing frequently queried columns — such as Seller TIN, Buyer TIN, and Item Catalog Code — the time increased to about 70 seconds. This was a worthwhile tradeoff since uploads happen infrequently, while read operations occur constantly and need to be fast.

The system supports rich, dynamic filtering and sorting using AJAX, allowing users to quickly drill down into large datasets without page reloads. Filters support a wide range of use cases — from date and number ranges to specific entity attributes like TINs, product codes, and source files. Users can apply multiple filters simultaneously, which makes narrowing down millions of records fast and intuitive.

Architecturally, the project was built using the Code First approach, which allowed me to define the data structure flexibly through C# classes and manage database migrations directly in code. This accelerated the modeling and development process, especially in the early stages when business rules were still being clarified. This approach also simplified automatic schema generation and versioning, making the project easier to maintain and extend.

Beyond filtering, I developed a custom analytics module designed to meet deeper business needs the client had described during discussions. They wanted to identify their best-performing products in different seasons, detect which buyers bring the most revenue, and uncover any suspicious reseller activity by tracing how goods moved between companies. To support that, I implemented global and filtered analytics modes. For example, with just a few clicks, users can see the top 30 most sold items, the most valuable sales, or even the highest-spending buyers — either across all time or within a filtered timeframe or dataset.

This approach essentially translated the client’s high-level operational concerns into actionable data insights, directly within the interface — no more relying on Excel macros or slow manual lookups. The logic was designed to be fast, extendable, and tightly aligned with real-world business questions the client needed to answer.

Although the UI was kept minimal, the core logic and performance were the priorities — and ultimately what convinced the client to bring me onto the full project.
