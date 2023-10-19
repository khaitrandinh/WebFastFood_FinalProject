create database finalProject_ASP
go
use finalProject_ASP
go

create table foods
(
food_id int identity(1,1) primary key not null,
food_name nvarchar(50) not null,
image varchar(50),
price decimal(18,0),
update_date smalldatetime,
quantity_instock int
)
go
create table customers(
customer_id int identity(1,1) primary key,
customer_name nvarchar(50),
username varchar(20),
password varchar(10),
email varchar(30),
address nvarchar(100),
numberphone varchar(12),
dob smalldatetime
)
go
create table orders(
order_id int identity(1,1) primary key,
ispayment bit,
isship bit,
order_date date,
delivery_date date,
customer_id int references customers(customer_id)
)
go
create table orderdetails(
order_id int references orders(order_id),
food_id int references foods(food_id),
quantity int,
price decimal(18,0),
primary key(order_id,food_id)
)
go
 

go
-- Create a table for admins
create table admins
(
    admin_id int identity(1,1) primary key not null,
    admin_name nvarchar(50),
    admin_username varchar(20),
    admin_password varchar(10),
    admin_email varchar(30)
)
go

alter table orders
add admin_id int references admins(admin_id)
go
alter table foods
add info nvarchar(100)
go
ALTER TABLE customers
ALTER COLUMN password VARCHAR(100) COLLATE Latin1_General_CS_AS;
go
ALTER TABLE admins
ALTER COLUMN admin_password VARCHAR(100) COLLATE Latin1_General_CS_AS;
go

-- Insert sample data into admins table
insert into admins(admin_name, admin_username, admin_password, admin_email)
values(N'Hữu Thắng', 'admin', 'thang123', 'HuuThang030603@gmail.com')

delete from orders;