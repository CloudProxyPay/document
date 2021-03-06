<?php

$apiKey = '';
//网关地址，请联系客服
$host = '';
$merchant_no = "";

echo "\n";
print_r("==================订单创建==================");
#注意：fundAccountNo与fundAccountUser下发提现订单必填，充值订单不填
$array = array(
    'orderNo'=>'00000000000001',
    'merchantNo'=>$merchant_no,
    'amount'=>1000,
    'payMode'=>'ebank',
    'orderType'=>'ISSUED',
    'ts'=>time(),
    'notifyUrl'=>'https://www.baidu.com/notify',
    'fundAccountNo'=>'6217003260005288000',
    'fundAccountUser'=>'张三',
    'fundAccountName'=>'中国银行');
print_r($array);
echo "\n";
$sign_reduce=generate_sign_reduce($array);
echo "sign_reduce:".$sign_reduce;
echo "\n";
$sign = md5($sign_reduce.'key='.$apiKey);
echo "sign:".$sign;
//向数组里添加sign
$array['sign']=$sign;
echo "\n";
print_r($array);
#链接跳转形式
$create_order_link = $host.'/cloud-order/#/create?'.$sign_reduce.'sign='.$sign;
echo "创建订单链接------>".$create_order_link;


echo "\n\n";
print_r("==================订单查询==================");

$url = $host.'/cloud-pay/open/order/query';
$array = array(
    'merchantNo'=>$merchant_no,
    'orderNo'=>'00000000000001',
    'ts'=>time()
    );
print_r($array);

$sign_reduce=generate_sign_reduce($array);
echo "sign_reduce:".$sign_reduce;
echo "\n";
$sign = md5($sign_reduce.'key='.$apiKey);
echo "sign:".$sign;

//向数组里添加sign
$array['sign']=$sign;
print_r($array);

$result = json_post($array,$url);
echo"订单查询结果:".$result;

function json_post($array,$url){
    //转json
    $params = json_encode($array);
    //使用CURL发起psot请求 
    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL, $url);
    curl_setopt($ch, CURLOPT_HTTPHEADER, array(
        'Content-Type: application/json',
        'Content-Length: ' . strlen($params)
    ));
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    curl_setopt($ch, CURLOPT_BINARYTRANSFER, true);
    curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "POST");
    curl_setopt($ch, CURLOPT_POSTFIELDS, $params);
     
    $result = curl_exec($ch);
    return $result;
    curl_close($ch);
}

function generate_sign_reduce($array){
//按键顺序正序排序
ksort($array);
//拼接 
$original_str = '';
foreach ($array as $key=>$value) {
    if(!empty($value) && strcasecmp('sign',$key) != 0){
        $original_str.=$key.'='.urlencode($value).'&';
    }
}
    echo "\n";
    return $original_str;
    echo "original_str:".$original_str; 
}

echo "\n\n";
print_r("==================验证签名==================");
$json = '{
	"amount": 1000,
	"realAmount": 1000,
	"orderNo": "00000000000001",
	"systemOrderNo": "20200530155239426165489893",
	"merchantNo": "20200507105801957145298221",
	"orderType": "ISSUED",
	"payMode": "ebank",
	"fundAccountNo": "6217003260005288000",
	"fundAccountUser": "张三",
	"fundAccountName": "中国银行",
	"fundAccountUrl": null,
	"voucherUrl": "https://timgsa.baidu.com/timg.jpg",
	"payTime": 1590825459,
	"ts": 1590825128,
	"payStatus": 30,
	"orderStatus": 50,
	"sign": "7e1e646c32f42d12d9aa303d131e30cb"
}';
//将json串转化成数组
$verify_array=json_decode($json,true);
print_r($verify_array);
echo "\n";
//获取sign值
foreach($verify_array as $key=>$value){
    if($key=='sign'){
       $get_sign= $value;
       print_r($get_sign);
    }
}
echo "\n";
$sign_reduce=generate_sign_reduce($verify_array);
echo "sign_reduce:".$sign_reduce;
echo "\n";
$sign = md5($sign_reduce.'key='.$apiKey);
echo "sign:".$sign;

echo "\n";
if(strcasecmp($get_sign,$sign) == 0){
     echo "sign_verify success";
}else{
     echo "sign_verify fail";
};


?>
