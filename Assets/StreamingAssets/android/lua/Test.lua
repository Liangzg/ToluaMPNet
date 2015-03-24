--��namespace�������Ͷ���
require "Golbal"

--���Լ�����������
function Test(transform)	
	local t = Time.time
	
	local v = Vector3.one
	local one = Vector3.one

	for i = 1,800000 do
		v = transform.position
		v:Add(one)
		transform.position = v
	end

	print("lua cost time: ", Time.time - t)
end

function Test2()	
	local t = Time.time	
	local v = Vector3.one
	local one = Vector3.one

	for i = 1,800000 do		
		v:Add(one)		
	end

	print("lua cost time: ", Time.time - t, v)
end

function FixedUpdate()
	print("lua FixedUpdate ")
end

--���Բ���������
print("Test Vector3 operator func")
local v1 = Vector3(1,2,3)
v1 = v1 + Vector3.one
print("Vector3 value is:" .. tostring(v1))

--֧��table���ƹ��캯��
local go = GameObject("Testenum")
--����ö�ٺ�numberֵ���غ���
go.transform:Rotate(Vector3.one, Space.Self)
go.transform:Rotate(Vector3.up, 12.5)

local go = GameObject("Light")
--����Ļ�ȡ������Ϣ ����.GetClassType
local lt = go:AddComponent(Light.GetClassType())
--��һ��number����ת��Ϊö��
lt.type = LightType.IntToEnum(1)

--ö�ٱȽ�
if lt.type == LightType.Directional then
	print("we have a directional light")
end



