#include <QmlNet/types/NetTypeArrayFacadeArray.h>
#include <QmlNet/types/NetTypeInfo.h>
#include <QmlNet/types/NetMethodInfo.h>
#include <QmlNet/types/NetPropertyInfo.h>
#include <QmlNet/qml/NetVariant.h>
#include <QmlNet/qml/NetVariantList.h>
#include <QmlNet/types/Callbacks.h>
#include <QmlNet/types/NetTypeManager.h>
#include <QDebug>

NetTypeArrayFacade_Array::NetTypeArrayFacade_Array(const QSharedPointer<NetTypeInfo>& type) :
    _isIncomplete(false)
{
    QSharedPointer<NetTypeInfo> arrayType = type;

    while(arrayType != nullptr && arrayType->getClassName() != "Array") {
        arrayType = NetTypeManager::getBaseType(arrayType);
    }

    if(arrayType == nullptr) {
        _isIncomplete = true;
        qWarning() << "Couldn't get the base array type for" << type->getClassName();
        return;
    }

    for(int x = 0; x < arrayType->getPropertyCount(); x++) {
        QSharedPointer<NetPropertyInfo> property = arrayType->getProperty(x);
        if(property->getPropertyName().compare("Length") == 0) {
            _lengthProperty = property;
        }
    }

    for(int x = 0; x < type->getMethodCount(); x++) {
        QSharedPointer<NetMethodInfo> method = type->getMethodInfo(x);
        if(method->getMethodName().compare("Get") == 0) {
            _getIndexed = method;
        } else if(method->getMethodName().compare("Set") == 0) {
            _setIndexed = method;
        }
    }

    if(_lengthProperty == nullptr ||
        _getIndexed == nullptr ||
        _setIndexed == nullptr) {
        _isIncomplete = true;
        qWarning() << "Couldn't all the array methods/properties for" << type->getClassName();
        return;
    }
}

bool NetTypeArrayFacade_Array::isIncomplete()
{
    return _isIncomplete;
}

bool NetTypeArrayFacade_Array::isFixed()
{
    // You can't change the size of an array.
    return true;
}

uint NetTypeArrayFacade_Array::getLength(const QSharedPointer<NetReference>& reference)
{
    QSharedPointer<NetVariant> result(new NetVariant());
    QmlNet::readProperty(_lengthProperty, reference, nullptr, result);
    return static_cast<uint>(result->getInt());
}

QSharedPointer<NetVariant> NetTypeArrayFacade_Array::getIndexed(const QSharedPointer<NetReference>& reference, uint index)
{
    QSharedPointer<NetVariantList> parameters = QSharedPointer<NetVariantList>(new NetVariantList());
    QSharedPointer<NetVariant> parameter = QSharedPointer<NetVariant>(new NetVariant());
    parameter->setInt(static_cast<int>(index));
    parameters->add(parameter);
    QSharedPointer<NetVariant> result = QSharedPointer<NetVariant>(new NetVariant());
    QmlNet::invokeNetMethod(_getIndexed, reference, parameters, result);
    return result;
}

void NetTypeArrayFacade_Array::setIndexed(const QSharedPointer<NetReference>& reference, uint index, const QSharedPointer<NetVariant>& value)
{
    QSharedPointer<NetVariantList> parameters = QSharedPointer<NetVariantList>(new NetVariantList());
    QSharedPointer<NetVariant> parameter = QSharedPointer<NetVariant>(new NetVariant());
    parameter->setInt(static_cast<int>(index));
    parameters->add(parameter);
    parameters->add(value);
    QSharedPointer<NetVariant> result = QSharedPointer<NetVariant>(new NetVariant());
    QmlNet::invokeNetMethod(_setIndexed, reference, parameters, result);
}
