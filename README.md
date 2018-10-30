### System Description

![](https://github.com/rcl-lab-connector/docs/raw/master/.gitbook/assets/system.PNG)

### Internet of Things \(IoT\)

The Internet of Things \(IoT\) or Internet of Medical Things \(IoMT\) is a term used to describe the concept of devices connecting to a computer systems to transfer and receive data. Ultimately, this data is stored in a cloud system and is made available to users for viewing and analyzing. IoT supports Big Data Analysis, Machine Learning and Artificial Intelligence systems.

### **Health IoT Hub**

Health IoT Hub is a set of cloud-based REST APIs that decodes device messages, saves message data to a cloud database and provides access to the data to health software applications via REST APIs.

#### **Processing Service**

The processing service is a REST API that receives HL7, ASTM and POCT messages from the Health IoT Edge application and decodes this message to a format for further data processing. It returns the decoded message to the Health IoT Edge application as JSON.

#### **Database Storage Service**

The database storage service is a REST API that will receive decoded messages from the Health IoT Edge application and will save the data to a cloud database.

#### Security

The Health IoT Hub enforces a secured TLS/HTTPS connection for data security. In addition, the REST APIs can only be accessed through OAuth 2 authorized requests.

#### Consuming the REST APIs

The Health IoT Hub will provide access to the data in the cloud database through the Database Storage Service. This API can be consumed by desktop, web and mobile health software applications. The data can also be used for Big Data Analysis , Machine Learning and Artificial Intelligence.

