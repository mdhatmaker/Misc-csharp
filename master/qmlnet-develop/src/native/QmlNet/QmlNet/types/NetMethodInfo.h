#ifndef NET_TYPE_INFO_METHOD_H
#define NET_TYPE_INFO_METHOD_H

#include <QmlNet/types/NetTypeInfo.h>
#include <QSharedPointer>

class NetTypeInfo;

class NetMethodInfoArguement {
public:
    NetMethodInfoArguement(QString name, QSharedPointer<NetTypeInfo> type);
    QString getName();
    QSharedPointer<NetTypeInfo> getType();
private:
    QString _name;
    QSharedPointer<NetTypeInfo> _type;
};

class NetMethodInfo {
public:
    NetMethodInfo(QSharedPointer<NetTypeInfo> parentTypeInfo,
                  QString methodName,
                  QSharedPointer<NetTypeInfo> returnType,
                  bool isStatic);

    int getId();

    QSharedPointer<NetTypeInfo> getParentType();

    QString getMethodName();

    QSharedPointer<NetTypeInfo> getReturnType();

    bool isStatic();

    void addParameter(QString name, QSharedPointer<NetTypeInfo> typeInfo);
    int getParameterCount();
    QSharedPointer<NetMethodInfoArguement> getParameter(int index);

    QString getSignature();

private:
    int _id;
    QSharedPointer<NetTypeInfo> _parentTypeInfo;
    QString _methodName;
    QSharedPointer<NetTypeInfo> _returnType;
    bool _isStatic;
    QList<QSharedPointer<NetMethodInfoArguement>> _parameters;
};

struct NetMethodInfoContainer {
    QSharedPointer<NetMethodInfo> method;
};

struct NetMethodInfoArguementContainer {
    QSharedPointer<NetMethodInfoArguement> methodArguement;
};

#endif // NET_TYPE_INFO_METHOD_H
